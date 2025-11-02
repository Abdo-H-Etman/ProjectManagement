namespace Domain.Entities.Models;

public class TaskTag
{
    public Guid TaskId { get; set; }
    public Guid TagId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Task Task { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}
