namespace Domain.Entities.Models;

public class ProjectTag
{
    public Guid ProjectId { get; set; }
    public Guid TagId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Project Project { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}
