using Domain.Entities.Enums;

namespace Domain.Entities.Models;

public class ProjectInvitation : BaseEntity
{
    public Guid ProjectId { get; set; }
    public required string Email { get; set; }
    public MemberRole Role { get; set; } = MemberRole.Member;
    public Guid InvitedById { get; set; }
    public required string Token { get; set; } = Guid.NewGuid().ToString();
    public bool IsAccepted { get; set; } = false;
    public DateTime? AcceptedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public Project Project { get; set; } = null!;
    public ApplicationUser InvitedBy { get; set; } = null!;
}
