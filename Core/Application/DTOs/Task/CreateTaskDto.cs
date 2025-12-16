using Domain.Entities.Enums;
using TaskStatus = Domain.Entities.Enums.TaskStatus;

namespace Application.DTOs.Task;

public record CreateTaskDto
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public TaskStatus Status { get; init; } = TaskStatus.Pending;
    public TaskPriority Priority { get; init; } = TaskPriority.Medium;
    public DateTime? DueDate { get; init; }
}
