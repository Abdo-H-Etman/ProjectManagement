using Domain.Entities.Enums;
using TaskStatus = Domain.Entities.Enums.TaskStatus;

namespace Application.DTOs.Task;

public record CreateTaskDto
{
    public Guid ProjectId { get; init; }
    public Guid? ParentTaskId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Status { get; init; } = TaskStatus.Pending.ToString();
    public string Priority { get; init; } = TaskPriority.Medium.ToString();
    public DateTime? DueDate { get; init; }
}
