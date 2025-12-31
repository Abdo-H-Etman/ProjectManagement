
using Application.Common.Models;
using Application.DTOs.User;

namespace Application.Interfaces;

public interface IUserService
{
    Task<Result<UserDto>> GetUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
    Task<Result<UserDetailsDto>> GetUserDetailsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);    
    Task<Result<UserProfileDto>> GetUserProfileAsync(
        Guid userId,
        CancellationToken cancellationToken = default);    
    Task<Result<IEnumerable<UserDto>>> GetAllUsersAsync(
        CancellationToken cancellationToken = default);
    Task<Result<UserDto>> UpdateUserAsync(
        Guid userId,
        UpdateUserDto updateUserDto,
        CancellationToken cancellationToken = default);
    Task<Result<UserProfileDto>> UpdateUserProfileAsync(
        Guid userId,
        UserProfileDto userProfileDto,
        CancellationToken cancellationToken = default);    
    Task<Result> DeactivateUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
    Task<Result> ActivateUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
    Task<Result> DeleteUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}