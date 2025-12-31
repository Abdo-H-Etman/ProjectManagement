namespace Application.DTOs.User;

public record UserProfileDto
{
    public string? Bio { get; init; }
    public string? AvatarUrl { get; init; }
    public string? Location { get; init; }
    public string? Department { get; init; }
    public string? JobTitle { get; init; }
    public string? PhoneNumber { get; init; }
}
