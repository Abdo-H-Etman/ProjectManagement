
using Domain.Entities.Models;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TaskStatus = Domain.Entities.Enums.TaskStatus;

namespace Infrastructure.Repositories;

public class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Project?> GetProjectWithDetailsAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbSet
            .Include(p => p.Owner)
            .Include(p => p.Members)
                .ThenInclude(pm => pm.User)
            .Include(p => p.Tasks.Where(t => !t.IsDeleted))
            .Include(p => p.ProjectTags)
                .ThenInclude(pt => pt.Tag)
            .Include(p => p.Attachments)
            .AsNoTracking()
            .Where(p => !p.IsDeleted && p.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IEnumerable<Project>> GetUserProjectsAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .Include(p => p.Owner)
            .AsNoTracking()
            .Where(p => !p.IsDeleted
                    && !p.IsArchived
                    &&( p.OwnerId == userId || p.Members.Any(pm => pm.UserId == userId && pm.IsActive)))
            .OrderByDescending(p => p.UpdatedAt)
            .ToListAsync(cancellationToken);
    
    public async Task<IEnumerable<Project>> GetArchivedProjectsAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .Include(p => p.Owner)
            .AsNoTracking()
            .Where(p => !p.IsDeleted
                    && p.IsArchived
                    && (p.OwnerId == userId || p.Members.Any(pm => pm.UserId == userId)))
            .OrderByDescending(p => p.UpdatedAt)
            .ToListAsync(cancellationToken);
    
    public async Task<IEnumerable<Project>> SearchProjectsAsync(string searchTerm, Guid userId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .Include(p => p.Owner)
            .AsNoTracking()
            .Where(p => !p.IsDeleted
                    && (p.OwnerId == userId || p.Members.Any(pm => pm.UserId == userId && pm.IsActive))
                    && (p.Name.Contains(searchTerm) || p.Description!.Contains(searchTerm)))
            .OrderByDescending(p => p.UpdatedAt)
            .ToListAsync(cancellationToken);
    
    public async Task<bool> IsUserProjectMemberAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .AsNoTracking()
            .Where(p => !p.IsDeleted && p.Id == projectId)
            .AnyAsync(p => p.OwnerId == userId || p.Members.Any(pm => pm.UserId == userId && pm.IsActive), cancellationToken);
    
    public async Task<ProjectStatistics> GetProjectStatisticsAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var project = await _dbSet
            .Include(p => p.Tasks.Where(t => !t.IsDeleted))
            .Include(p => p.Members.Where(m => m.IsActive))
            .AsNoTracking()
            .FirstOrDefaultAsync(p => !p.IsDeleted && p.Id == projectId, cancellationToken);

        if(project == null)
            return new ProjectStatistics(); 

        var tasks = project.Tasks.ToList();
        var totaTasks = tasks.Count;
        var completedTasks = tasks.Count(t => t.Status == TaskStatus.Completed);

        return new ProjectStatistics
        {
            TotalTasks = totaTasks,
            CompletedTasks = completedTasks,
            InProgressTasks = tasks.Count(t => t.Status == TaskStatus.InProgress),
            PendingTasks = tasks.Count(t => t.Status == TaskStatus.Pending),
            OverdueTasks = tasks.Count(t => t.DueDate.HasValue && t.DueDate < DateTime.UtcNow && t.Status != TaskStatus.Completed),
            CompletionPercentage = totaTasks == 0 ? 0 : (decimal)completedTasks / totaTasks * 100,
            TotalMembers = project.Members.Count,
            TotalEstimatedHours = tasks.Sum(t => t.EstimatedHours ?? 0),
            TotalActualHours = tasks.Sum(t => t.ActualHours ?? 0)
        };
    }


}
