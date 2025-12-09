using Domain.Entities.Models;
using Task = System.Threading.Tasks.Task;

namespace Domain.Interfaces;

public interface IRepositoryManager : IDisposable
{
    IApplicationUserRepository User { get; }
    IProjectRepository Project { get; }
    ITaskRepository Task { get; }
    IActivityLogRepository ActivityLog { get; }
    INotificationRepository Notification { get; }
    IAttachmentRepository Attachment { get; }
    ICommentRepository Comment { get; }
    IRepository<ApiKey> ApiKey { get; }
    IRepository<CheckListItem> CheckListItem { get; }
    IRepository<ProjectInvitation> ProjectInvitation { get; }
    IRepository<Tag> Tag { get; }
    IRepository<TimeEntry> TimeEntry { get; }
    IRepository<UserProfile> UserProfile { get; }
    IRepository<Webhook> Webhook { get; }

    Task SaveAsync(CancellationToken cancellationToken = default);
}
