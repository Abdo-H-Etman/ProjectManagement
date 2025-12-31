
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Common.Models;
using Application.Common.Models.Interfaces;
using Application.Common.Settings;
using Application.DTOs.Auth;
using Application.DTOs.User;
using Application.Interfaces.Auth;
using Application.Interfaces.Logging;
using Application.Interfaces.Mailing;
using Domain.Entities.Models;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services.Auth;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEmailService _emailService;
    private readonly IRepositoryManager _repositoryManager;
    private readonly JwtSettings _jwtSettings;
    private readonly ILoggerManager _logger;

    public AuthenticationService(
        ICurrentUserService currentUserService,
        IRepositoryManager repositoryManager,
        IOptions<JwtSettings> jwtSettings,
        ILoggerManager logger,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailService emailService)
    {
        _currentUserService = currentUserService;
        _repositoryManager = repositoryManager;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
        _emailService = emailService;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<Result<AuthResponseDto>> RegisterUserAsync(
        CreateUserDto createUserDto,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(createUserDto.Email);
            if (existingUser != null)
            {
                return Result<AuthResponseDto>.Failure("Email is already registered.");
            }

            existingUser = await _userManager.FindByNameAsync(createUserDto.UserName);
            if (existingUser != null)
            {
                return Result<AuthResponseDto>.Failure("Username is already taken.");
            }

            if (createUserDto.Password != createUserDto.ConfirmPassword)
            {
                return Result<AuthResponseDto>.Failure("Passwords do not match.");
            }

            var newUser = new ApplicationUser
            {
                UserName = createUserDto.UserName,
                Email = createUserDto.Email,
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName
            };

            var result = await _userManager.CreateAsync(newUser, createUserDto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return Result<AuthResponseDto>.Failure($"User creation failed.", errors);
            }

            var createdUser = await _userManager.FindByEmailAsync(createUserDto.Email);
            if (createdUser == null)
            {
                return Result<AuthResponseDto>.Failure("User was created but could not be retrieved from database.");
            }

            var authResponse = await GenerateAuthResponseDtoAsync(createdUser, ipAddress, userAgent, cancellationToken);

            var profile = new UserProfile
            {
                UserId = newUser.Id,
            };
            await _repositoryManager.UserProfile.AddAsync(profile, cancellationToken);
            await _repositoryManager.SaveAsync(cancellationToken);

            _logger.LogInfo("User registered successfully.");
            return Result<AuthResponseDto>.Success(authResponse, "User registered successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during user registration: {ex.Message}");
            return Result<AuthResponseDto>.Failure("Registration failed.");
        }
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(
        LoginDto loginDto,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(loginDto.Identifier)
                       ?? await _userManager.FindByEmailAsync(loginDto.Identifier);
            if (user == null)
            {
                return Result<AuthResponseDto>.Failure("Invalid credentials.");
            }

            if (!user.IsActive)
            {
                return Result<AuthResponseDto>.Failure("User account is inactive.");
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: true);
            if (signInResult.IsLockedOut)
            {
                return Result<AuthResponseDto>.Failure("User account is locked out. please try again later.");
            }
            if (!signInResult.Succeeded)
            {
                return Result<AuthResponseDto>.Failure("Invalid credentials.");
            }

            var authResponse = await GenerateAuthResponseDtoAsync(user, ipAddress, userAgent, cancellationToken);
            user.LastLoginAt = DateTime.UtcNow;
            _logger.LogInfo("User logged in successfully.");
            return Result<AuthResponseDto>.Success(authResponse, "User logged in successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during user login: {ex.Message}");
            return Result<AuthResponseDto>.Failure("Login failed.");
        }
    }

    public async Task<Result<AuthResponseDto>> RefreshTokenAsync(
        TokenDto tokenDto,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);
            if (principal == null)
            {
                return Result<AuthResponseDto>.Failure("Invalid access token.");
            }

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Result<AuthResponseDto>.Failure("Invalid token claims.");
            }

            var refreshToken = await _repositoryManager.RefreshToken.FirstOrDefaultAsync(
                rt => rt.Token == tokenDto.RefreshToken && rt.UserId == userId && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow,
                cancellationToken);

            if (refreshToken == null)
            {
                return Result<AuthResponseDto>.Failure("Invalid or expired refresh token.");
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || !user.IsActive)
            {
                return Result<AuthResponseDto>.Failure("User not found or inactive.");
            }

            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;

            var authResponse = await GenerateAuthResponseDtoAsync(user, ipAddress, userAgent, cancellationToken);
            refreshToken.ReplacedByToken = authResponse.RefreshToken;

            _repositoryManager.RefreshToken.Update(refreshToken);
            await _repositoryManager.SaveAsync(cancellationToken);

            _logger.LogInfo($"Token refreshed successfully for user: {userId}.");
            return Result<AuthResponseDto>.Success(authResponse, "Token refreshed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during token refresh: {ex.Message}");
            return Result<AuthResponseDto>.Failure("Token refresh failed.");
        }
    }

    public async Task<Result> RevokeTokenAsync(
        string refreshToken,
        string? ipAddress = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await _repositoryManager.RefreshToken.FirstOrDefaultAsync(
                rt => rt.Token == refreshToken && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow,
                cancellationToken);

            if (token == null)
            {
                return Result.Failure("Invalid or already revoked refresh token.");
            }

            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;

            _repositoryManager.RefreshToken.Update(token);
            await _repositoryManager.SaveAsync(cancellationToken);

            _logger.LogInfo($"Refresh token revoked successfully for user: {token.UserId}.");
            return Result.Success("Refresh token revoked successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during token revocation: {ex.Message}");
            return Result.Failure("Token revocation failed.");
        }
    }

    public async Task<Result> LogoutAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var activeTokens = await _repositoryManager.RefreshToken
                .FindAsync(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow, cancellationToken);

            if (!activeTokens.Any())
            {
                return Result.Failure("User is not currently logged in.");
            }
            
            foreach (var token in activeTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
                _repositoryManager.RefreshToken.Update(token);
            }

            await _repositoryManager.SaveAsync(cancellationToken);

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                await _userManager.UpdateSecurityStampAsync(user);
            }

            _logger.LogInfo($"User {userId} logged out successfully.");
            return Result.Success("User logged out successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during user logout: {ex.Message}");
            return Result.Failure($"Logout failed. {ex.Message}");
        }
    }

    public async Task<Result> ChangePasswordAsync(
        Guid userId,
        ChangePasswordDto changePasswordDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
            {
                return Result.Failure("passwords do not match.");
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return Result.Failure("User not found.");
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return Result.Failure($"Password change failed.", errors);
            }

            _logger.LogInfo($"Password changed successfully for user: {userId}.");
            return Result.Success("Password changed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during password change: {ex.Message}");
            return Result.Failure("Password change failed.");
        }
    }

    public async Task<Result> ForgotPasswordAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Result.Failure("If the email is registered, password reset instructions will be sent.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = $"https://frontend.com/reset-password?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";
            var emailResult = await _emailService.SendPasswordResetEmailAsync(
                user.Email!,
                user.UserName ?? "User",
                token,
                resetLink,
                cancellationToken);

            if (!emailResult.IsSuccess)
            {
                _logger.LogError($"Failed to send password reset email to {user.Email}: {string.Join(';', emailResult.Errors)}");
                return Result.Failure("Failed to send password reset email.", emailResult.Errors);
            }

            _logger.LogInfo($"Password reset requested for user: {user.Id}.");
            return Result.Success("Password reset instructions sent to email.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during password reset process: {ex.Message}");
            return Result.Failure("An error occurred while processing your request.");
        }
    }

    public async Task<Result> ResetPasswordAsync(
        ResetPasswordDto resetPasswordDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmNewPassword)
            {
                return Result.Failure("Passwords do not match.");
            }

            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return Result.Failure("Invalid password reset request.");
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return Result.Failure("Password reset failed.", errors);
            }

            _logger.LogInfo($"Password reset successfully for: {resetPasswordDto.Email}.");
            return Result.Success("Password reset successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during password reset: {ex.Message}");
            return Result.Failure("Password reset failed.");
        }
    }

    private async Task<AuthResponseDto> GenerateAuthResponseDtoAsync(
            ApplicationUser user,
            string? ipAddress,
            string? userAgent,
            CancellationToken cancellationToken)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = await GenerateRefreshTokenAsync(user.Id, ipAddress, userAgent, cancellationToken);

        var profile = await _repositoryManager.UserProfile.FirstOrDefaultAsync(
            p => p.UserId == user.Id,
            cancellationToken);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            User = MapToUserResponse(user, profile)
        };
    }

    private string GenerateAccessToken(ApplicationUser user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName ?? ""),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim("SecurityStamp", user.SecurityStamp ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<RefreshToken> GenerateRefreshTokenAsync(
        Guid userId,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken)
    {
        try
        {
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = GenerateRefreshTokenString(),
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays),
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            var previousTokens = await _repositoryManager.RefreshToken
                .FindAsync(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow, cancellationToken);
            foreach (var token in previousTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
                token.ReplacedByToken = refreshToken.Token;
                _repositoryManager.RefreshToken.Update(token);
            }
            await _repositoryManager.RefreshToken.AddAsync(refreshToken, cancellationToken);
            await _repositoryManager.SaveAsync(cancellationToken);

            return refreshToken;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error generating refresh token: {ex.Message}");
            _logger.LogError($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    private string GenerateRefreshTokenString()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = false, // Don't validate lifetime for expired tokens
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }

    private UserDto MapToUserResponse(ApplicationUser user, UserProfile? profile)
    {
        return new UserDto
        {
            Id = user.Id,
            FullName = $"{user.FirstName} {user.LastName}",
            UserName = user.UserName ?? "",
            Email = user.Email ?? "",
            AvatarUrl = profile?.AvatarUrl,
        };
    }
}