using System.Security.Claims;
using Application.DTOs.Auth;
using Application.DTOs.User;
using Application.Interfaces.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public AuthenticationController(
        IAuthenticationService authenticationService,
        IHttpContextAccessor httpContextAccessor)
    {
        _authService = authenticationService;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] CreateUserDto createUserDto, CancellationToken cancellationToken)
    {
        var ipAddress = GetIpAddress();
        var userAgent = GetUserAgent();

        var result = await _authService.RegisterUserAsync(
            createUserDto,
            ipAddress,
            userAgent,
            cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto loginUserDto, CancellationToken cancellationToken)
    {
        var ipAddress = GetIpAddress();
        var userAgent = GetUserAgent();

        var result = await _authService.LoginAsync(
            loginUserDto,
            ipAddress,
            userAgent,
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var result = await _authService.LogoutAsync(userId, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var result = await _authService.ChangePasswordAsync(userId, changePasswordDto, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] string email,  CancellationToken cancellationToken)
    {
        var result = await _authService.ForgotPasswordAsync(email, cancellationToken);

        return Ok(result);
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto, CancellationToken cancellationToken)
    {
        var result = await _authService.ResetPasswordAsync(resetPasswordDto, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }


    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException());
    }
    private string? GetIpAddress()
    {
        return _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
    }

    private string? GetUserAgent()
    {
        return _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();
    }
}
