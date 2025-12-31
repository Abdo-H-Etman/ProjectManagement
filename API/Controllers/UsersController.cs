
using Application.DTOs.User;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetUser(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _userService.GetUserAsync(userId, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var result = await _userService.GetAllUsersAsync(cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("details/{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetUserDetails(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _userService.GetUserDetailsAsync(userId, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPatch("{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateUser(
        Guid userId,
        [FromBody] UpdateUserDto updateUserDto,
        CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateUserAsync(
            userId,
            updateUserDto,
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteUser(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _userService.DeleteUserAsync(userId, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("deactivate/{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeactivateUser(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _userService.DeactivateUserAsync(userId, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("activate/{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> ActivateUser(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _userService.ActivateUserAsync(userId, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("userprofile/{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetUserProfile(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _userService.GetUserProfileAsync(userId, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPatch("userprofile/{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateUserProfile(
        Guid userId,
        [FromBody] UserProfileDto userProfileDto,
        CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateUserProfileAsync(
            userId,
            userProfileDto,
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }


}