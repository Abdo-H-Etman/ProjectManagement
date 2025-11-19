namespace Domain.Entities.Models;

public class TaskDependency
{
    public Guid TaskId { get; set; }
    public Guid DependsOnTaskId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Task Task { get; set; } = null!;
    public Task DependsOnTask { get; set; } = null!;
}
