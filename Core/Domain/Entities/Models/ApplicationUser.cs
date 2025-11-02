using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public bool IsActive { get; set; } = true;
    public DateTime LastLoginAt { get; set; }
    public string TimeZone { get; set; } = "UTC";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public UserProfile? UserProfile { get; set; }
    public ICollection<Project> OwnedProjects { get; set; } = [];
    public ICollection<ProjectMember> ProjectMemberships { get; set; } = [];
    public ICollection<Task> AssignedTasks { get; set; } = [];
    public ICollection<Task> CreatedTasks { get; set; } = [];
    public ICollection<Attachment> Attachments { get; set; } = [];
    public ICollection<Comment> Comments { get; set; } = [];
    public ICollection<TimeEntry> TimeEntries { get; set; } = [];
    public ICollection<Notification> Notifications { get; set; } = [];
    public ICollection<UserFavorite> Favorites { get; set; } = [];
}
