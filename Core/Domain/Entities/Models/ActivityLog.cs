using Domain.Entities.Enums;

namespace Domain.Entities.Models;

public class ActivityLog : BaseEntity
{
    public Guid? TaskId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid UserId { get; set; }
    public ActivityType Type { get; set; }
    public required string Description { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }

    public Task? Task { get; set; }
    public Project? Project { get; set; }
    public ApplicationUser User { get; set; } = null!;
}
