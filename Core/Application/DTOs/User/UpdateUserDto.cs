namespace Application.DTOs.User;

public record UpdateUserDto
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? UserName { get; init; }
}
