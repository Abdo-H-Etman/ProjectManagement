using Domain.Entities.Models;

namespace Domain.Interfaces;

public interface IProjectRepository : IRepository<Project>
{
    Task<Project?> GetProjectWithDetailsAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetUserProjectsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetArchivedProjectsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> SearchProjectsAsync(string searchTerm, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IsUserProjectMemberAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default);
    Task<ProjectStatistics> GetProjectStatisticsAsync(Guid projectId, CancellationToken cancellationToken = default);
}
public class ProjectStatistics
{
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int InProgressTasks { get; set; }
    public int PendingTasks { get; set; }
    public int OverdueTasks { get; set; }
    public decimal CompletionPercentage { get; set; }
    public int TotalMembers { get; set; }
    public decimal TotalEstimatedHours { get; set; }
    public decimal TotalActualHours { get; set; }
}