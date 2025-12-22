namespace Application.DTOs.User;

public record UserDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string UserName { get; init; } = null!;
    public string? AvatarUrl { get; init; }
}
