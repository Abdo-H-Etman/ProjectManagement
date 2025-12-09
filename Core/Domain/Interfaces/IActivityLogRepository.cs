using Domain.Entities.Models;

namespace Domain.Interfaces;

public interface IActivityLogRepository : IRepository<ActivityLog>
{
    Task<IEnumerable<ActivityLog>> GetProjectActivityAsync(Guid projectId, int limit = 50, CancellationToken cancellationToken = default);
    Task<IEnumerable<ActivityLog>> GetTaskActivityAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ActivityLog>> GetUserActivityAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<ActivityStatistics> GetActivityStatisticsAsync(Guid projectId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
}

public class ActivityStatistics
{
    public int TotalActivities {get; set;}
    public Dictionary<string, int> ActivitiesByType {get; set;} = [];
    public Dictionary<DateTime, int> ActivitiesByDate {get; set;} = [];
    public List<UserActivitySummary> TopActiveUsers {get; set;} = [];
}

public class UserActivitySummary
{
    public Guid UserId {get; set;}
    public string UserName {get; set;} = string.Empty;
    public int ActivityCount {get; set;}
}