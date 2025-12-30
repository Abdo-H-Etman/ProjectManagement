using Domain.Entities.Enums;
using TaskStatus = Domain.Entities.Enums.TaskStatus;

namespace Application.DTOs.Task;

public record UpdateTaskDto
{
    public string? Title { get; init; }
    public string? Description { get; init; }
    public TaskStatus Status { get; init; }
    public TaskPriority Priority { get; init; }
    public DateTime? DueDate { get; init; }
}
