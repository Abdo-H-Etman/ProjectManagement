
using Application.Common.Models;
using Application.DTOs.Project;
using Application.DTOs.User;
using Application.Interfaces;
using Application.Interfaces.Logging;
using Domain.Interfaces;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly ILoggerManager _logger;
    private readonly IRepositoryManager _repository;
    public UserService(
        ILoggerManager logger,
        IRepositoryManager repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<Result<UserDto>> GetUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _repository.User.GetByIdAsync(
                userId,
                cancellationToken);

            if (user == null || user.IsDeleted)
            {
                return Result<UserDto>.Failure("User not found.");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                UserName = user.UserName!,
                Email = user.Email!,
                AvatarUrl = user.UserProfile?.AvatarUrl
            };

            return Result<UserDto>.Success(userDto, "User retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(GetUserAsync)}: {ex.Message}");
            return Result<UserDto>.Failure("An error occurred while retrieving the user.");
        }
    }
    
    public async Task<Result<UserDetailsDto>> GetUserDetailsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _repository.User.GetByIdAsync(
                userId,
                cancellationToken
            );

            if (user == null || user.IsDeleted)
            {
                return Result<UserDetailsDto>.Failure("User not found.");
            }
            var userDetailsDto = new UserDetailsDto
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                UserName = user.UserName!,
                Email = user.Email!,
                TimeZone = user.TimeZone,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                Profile = user.UserProfile == null ? null : new UserProfileDto
                {
                    AvatarUrl = user.UserProfile.AvatarUrl,
                    Bio = user.UserProfile.Bio,
                    Location = user.UserProfile.Location,
                    Department = user.UserProfile.Department,
                    JobTitle = user.UserProfile.JobTitle
                },
                Projects = [.. user.OwnedProjects.Select(p => new ProjectSummaryDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description
                })]
            };
            return Result<UserDetailsDto>.Success(userDetailsDto, "User details retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(GetUserDetailsAsync)}: {ex.Message}");
            return Result<UserDetailsDto>.Failure("An error occurred while retrieving the user details.");
        }
    }

    public async Task<Result<UserProfileDto>> GetUserProfileAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _repository.User.GetByIdWithProfileAsync(
                userId,
                cancellationToken
            );

            if (user == null || user.IsDeleted)
            {
                return Result<UserProfileDto>.Failure("User not found.");
            }

            var userProfile = await _repository.UserProfile.GetByUserIdAsync(userId, cancellationToken);

            if (userProfile == null)
            {
                return Result<UserProfileDto>.Failure("User profile not found.");
            }

            var userProfileDto = new UserProfileDto
            {
                AvatarUrl = userProfile.AvatarUrl,
                Bio = userProfile.Bio,
                Location = userProfile.Location,
                Department = userProfile.Department,
                JobTitle = userProfile.JobTitle
            };

            return Result<UserProfileDto>.Success(userProfileDto, "User profile retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(GetUserProfileAsync)}: {ex.Message}");
            return Result<UserProfileDto>.Failure("An error occurred while retrieving the user profile.");
        }
    }

    public async Task<Result<IEnumerable<UserDto>>> GetAllUsersAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await _repository.User.GetAllAsync(cancellationToken);

            var userDtos = users
                .Where(u => !u.IsDeleted)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FullName = $"{u.FirstName} {u.LastName}",
                    UserName = u.UserName!,
                    Email = u.Email!,
                    AvatarUrl = u.UserProfile?.AvatarUrl
                })
                .ToList();

            return Result<IEnumerable<UserDto>>.Success(userDtos, "Users retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(GetAllUsersAsync)}: {ex.Message}");
            return Result<IEnumerable<UserDto>>.Failure("An error occurred while retrieving users.");
        }
    }

    public async Task<Result<UserDto>> UpdateUserAsync(
        Guid userId,
        UpdateUserDto updateUserDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _repository.User.GetByIdAsync(
                userId,
                cancellationToken
            );
            if (user == null || user.IsDeleted)
            {
                return Result<UserDto>.Failure("User not found.");
            }

            user.FirstName = updateUserDto.FirstName ?? user.FirstName;
            user.LastName = updateUserDto.LastName ?? user.LastName;
            user.UserName = updateUserDto.UserName ?? user.UserName;

            _repository.User.Update(user);
            await _repository.SaveAsync(cancellationToken);

            var userDto = new UserDto
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                UserName = user.UserName!,
                Email = user.Email!,
                AvatarUrl = user.UserProfile?.AvatarUrl
            };

            return Result<UserDto>.Success(userDto, "User updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(UpdateUserAsync)}: {ex.Message}");
            return Result<UserDto>.Failure($"An error occurred while updating the user. {ex.Message}");
        }
    }

    public async Task<Result<UserProfileDto>> UpdateUserProfileAsync(
        Guid userId,
        UserProfileDto userProfileDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _repository.User.GetByIdAsync(
                userId,
                cancellationToken
            );
            if (user == null || user.IsDeleted)
            {
                return Result<UserProfileDto>.Failure("User not found.");
            }

            var userProfile = await _repository.UserProfile.GetByUserIdAsync(userId, cancellationToken);
            if (userProfile == null)
            {
                return Result<UserProfileDto>.Failure("User profile not found.");
            }

            userProfile.AvatarUrl = userProfileDto.AvatarUrl;
            userProfile.Bio = userProfileDto.Bio;
            userProfile.Location = userProfileDto.Location;
            userProfile.Department = userProfileDto.Department;
            userProfile.JobTitle = userProfileDto.JobTitle;
            userProfile.PhoneNumber = userProfileDto.PhoneNumber;

            _repository.UserProfile.Update(userProfile);
            await _repository.SaveAsync(cancellationToken);

            return Result<UserProfileDto>.Success(userProfileDto, "User profile updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(UpdateUserProfileAsync)}: {ex.Message}");
            return Result<UserProfileDto>.Failure("An error occurred while updating the user profile.");
        }
    }

    public async Task<Result> DeactivateUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _repository.User.GetByIdAsync(
                userId,
                cancellationToken
            );
            if (user == null || user.IsDeleted)
            {
                return Result.Failure("User not found.");
            }

            user.IsActive = false;
            _repository.User.Update(user);
            await _repository.SaveAsync(cancellationToken);

            return Result.Success("User deactivated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(DeactivateUserAsync)}: {ex.Message}");
            return Result.Failure("An error occurred while deactivating the user.");
        }
    }

    public async Task<Result> ActivateUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _repository.User.GetByIdAsync(
                userId,
                cancellationToken
            );
            if (user == null || user.IsDeleted)
            {
                return Result.Failure("User not found.");
            }

            user.IsActive = true;
            _repository.User.Update(user);
            await _repository.SaveAsync(cancellationToken);

            return Result.Success("User activated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(ActivateUserAsync)}: {ex.Message}");
            return Result.Failure("An error occurred while activating the user.");
        }
    }

    public async Task<Result> DeleteUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _repository.User.GetByIdAsync(
                userId,
                cancellationToken
            );
            if (user == null || user.IsDeleted)
            {
                return Result.Failure("User not found.");
            }

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            _repository.User.Update(user);
            await _repository.SaveAsync(cancellationToken);

            return Result.Success("User deleted successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {nameof(DeleteUserAsync)}: {ex.Message}");
            return Result.Failure("An error occurred while deleting the user.");
        }
    }

}
