using Domain.Entities.Enums;
using TaskStatus = Domain.Entities.Enums.TaskStatus;
using Task = Domain.Entities.Models.Task;

namespace Domain.Interfaces;

public interface ITaskRepository : IRepository<Task>
{
    Task<Task?> GetTaskWithDetailsAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Task>> GetProjectTasksAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Task>> GetUserTasksAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Task>> GetOverdueTasksAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Task>> GetTasksByStatusAsync(Guid projectId, TaskStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Task>> GetTasksByPriorityAsync(Guid projectId, TaskPriority priority, CancellationToken cancellationToken = default);
    Task<IEnumerable<Task>> GetSubtasksAsync(Guid parentTaskId, CancellationToken cancellationToken = default);
}