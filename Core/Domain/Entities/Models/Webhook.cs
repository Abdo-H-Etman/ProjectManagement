namespace Domain.Entities.Models;

public class Webhook : BaseEntity
{
    public Guid ProjectId { get; set; }
    public string Url { get; set; } = string.Empty;
    public required string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Secret { get; set; }
    public string Events { get; set; } = "[]";
    public int FailureCount { get; set; } = 0;
    public DateTime? LastTriggeredAt { get; set; }

    public Project Project { get; set; } = null!;
}
