namespace Application.DTOs.User;

public record UpdateUserDto
{
    public string FirstName { get; init; } = null!;
    public string? LastName { get; init; }
    public string UserName { get; init; } = null!;
    public string? PhoneNumber { get; init; }
    public string? Bio { get; init; }
    public string? Location { get; init; }
    public string? Department { get; init; }
    public string? JobTitle { get; init; }
}
