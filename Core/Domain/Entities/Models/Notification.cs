using Domain.Entities.Enums;

namespace Domain.Entities.Models;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public DateTime ReadAt { get; set; }
    public string? ActionUrl { get; set; }
    public string? Metadata { get; set; }

    public ApplicationUser User { get; set; } = null!;
}
