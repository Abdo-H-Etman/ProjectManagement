using Domain.Entities.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Task = Domain.Entities.Models.Task;
using TaskStatus = Domain.Entities.Enums.TaskStatus;

namespace Infrastructure.Repositories;

public class TaskRepository : Repository<Task>, ITaskRepository
{
    public TaskRepository(ApplicationDbContext context) : base(context)
    {  
    }

    public async Task<Task?> GetTaskWithDetailsAsync(Guid taskId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .Include(t => t.Project)
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .Include(t => t.ParentTask)
            .Include(t => t.Comments.Where(c => !c.IsDeleted))
                .ThenInclude(c => c.Author)
            .Include(t => t.SubTasks.Where(st => !st.IsDeleted))
            .Include(t => t.Attachments)
            .AsNoTracking()
            .Where(t => !t.IsDeleted && t.Id == taskId)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IEnumerable<Task>> GetProjectTasksAsync(Guid projectId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .AsNoTracking()
            .Where(t => !t.IsDeleted && t.ProjectId == projectId)
            .OrderByDescending(t => t.UpdatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Task>> GetUserTasksAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .AsNoTracking()
            .Where(t => !t.IsDeleted && t.AssignedToId == userId)
            .OrderByDescending(t => t.UpdatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Task>> GetOverdueTasksAsync(Guid projectId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .AsNoTracking()
            .Where(t => !t.IsDeleted 
                        && t.ProjectId == projectId
                        && t.DueDate.HasValue 
                        && t.DueDate < DateTime.UtcNow 
                        && t.Status != TaskStatus.Completed)
            .OrderByDescending(t => t.UpdatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Task>> GetTasksByStatusAsync(Guid projectId, TaskStatus status, CancellationToken cancellationToken = default) =>
        await _dbSet
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .AsNoTracking()
            .Where(t => !t.IsDeleted 
                        && t.ProjectId == projectId
                        && t.Status == status)
            .OrderByDescending(t => t.UpdatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Task>> GetTasksByPriorityAsync(Guid projectId, TaskPriority priority, CancellationToken cancellationToken = default) =>
        await _dbSet
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .AsNoTracking()
            .Where(t => !t.IsDeleted 
                        && t.ProjectId == projectId
                        && t.Priority == priority)
            .OrderByDescending(t => t.UpdatedAt)
            .ToListAsync(cancellationToken);
    
    public async Task<IEnumerable<Task>> GetSubtasksAsync(Guid parentTaskId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .AsNoTracking()
            .Where(t => !t.IsDeleted && t.ParentTaskId == parentTaskId)
            .OrderByDescending(t => t.UpdatedAt)
            .ToListAsync(cancellationToken);
}
