namespace Domain.Entities.Models;

public class UserProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public string? PhoneNumber { get; set; }
    public string? JobTitle { get; set; }
    public string? Department { get; set; }
    public string? Location { get; set; }
    public ApplicationUser User { get; set; } = null!;
}
