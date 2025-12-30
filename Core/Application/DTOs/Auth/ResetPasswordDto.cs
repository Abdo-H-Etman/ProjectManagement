namespace Application.DTOs.Auth;

public record ResetPasswordDto
{   
    public string Email { get; init; } = null!;
    public string Token { get; init; } = null!;
    public string NewPassword { get; init; } = null!;
    public string ConfirmNewPassword { get; init; } = null!;
}
