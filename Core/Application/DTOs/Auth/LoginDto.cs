namespace Application.DTOs.Auth;

public record LoginDto
{
    public string UserName { get; init; } = null!;
    public string Password { get; init; } = null!;
}
