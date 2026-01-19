namespace Application.DTOs.Project;

public record InvitationDto
{
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
