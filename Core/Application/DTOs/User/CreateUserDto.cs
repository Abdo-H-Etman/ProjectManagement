namespace Application.DTOs.User;

public record CreateUserDto
{
    public string FirstName { get; init; } = null!;
    public string? LastName { get; init; }
    public string Email { get; init; } = null!;
    public string UserName { get; init; } = null!;
    public string Password { get; init; } = null!;
}
