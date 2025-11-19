using Domain.Entities.Enums;

namespace Domain.Entities.Models;

public class Project : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;
    public Visibility Visibility { get; set; } = Visibility.Private;
    public Guid OwnerId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsArchived { get; set; } = false;
    public ApplicationUser Owner { get; set; } = null!;
    public ICollection<ProjectMember> Members { get; set; } = [];
    public ICollection<Task> Tasks { get; set; } = [];
    public ICollection<Attachment> Attachments { get; set; } = [];
    public ICollection<ProjectTag> ProjectTags { get; set; } = [];
    public ICollection<ActivityLog> ActivityLogs { get; set; } = [];
    public ICollection<UserFavorite> FavoritedBy { get; set; } = [];
}
