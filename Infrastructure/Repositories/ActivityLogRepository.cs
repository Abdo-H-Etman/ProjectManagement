using Domain.Entities.Models;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ActivityLogRepository : Repository<ActivityLog>, IActivityLogRepository
{
    public ActivityLogRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ActivityLog>> GetProjectActivityAsync(Guid projectId, int limit = 50, CancellationToken cancellationToken = default) =>
        await _dbSet
            .AsNoTracking()
            .Where(a => a.ProjectId == projectId)
            .OrderByDescending(a => a.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<ActivityLog>> GetTaskActivityAsync(Guid taskId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .AsNoTracking()
            .Where(a => a.TaskId == taskId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<ActivityLog>> GetUserActivityAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<ActivityStatistics> GetActivityStatisticsAsync(
    Guid projectId, 
    DateTime fromDate, 
    DateTime toDate, 
    CancellationToken cancellationToken = default)
    {
        var baseQuery = _dbSet
            .AsNoTracking()
            .Where(a => a.ProjectId == projectId 
                    && a.CreatedAt >= fromDate 
                    && a.CreatedAt <= toDate);

        var statistics = new ActivityStatistics
        {
            TotalActivities = await baseQuery.CountAsync(cancellationToken),
            
            ActivitiesByType = await baseQuery
                .GroupBy(a => a.Type)
                .Select(g => new { Type = g.Key.ToString(), Count = g.Count() })
                .ToDictionaryAsync(x => x.Type, x => x.Count, cancellationToken),
            
            ActivitiesByDate = await baseQuery
                .GroupBy(a => a.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Date, x => x.Count, cancellationToken),
            
            TopActiveUsers = await baseQuery
                .GroupBy(a => new { a.UserId, a.User.UserName })
                .Select(g => new UserActivitySummary
                {
                    UserId = g.Key.UserId,
                    UserName = g.Key.UserName ?? string.Empty,
                    ActivityCount = g.Count()
                })
                .OrderByDescending(u => u.ActivityCount)
                .Take(5)
                .ToListAsync(cancellationToken)
        };

        return statistics;
    }
            
}
