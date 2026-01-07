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
    IProjectInvitationRepository ProjectInvitation { get; }
    IProjectMemberRepository ProjectMember { get; }
    IRepository<ApiKey> ApiKey { get; }
    IRepository<CheckListItem> CheckListItem { get; }
    IRepository<Tag> Tag { get; }
    IRepository<TimeEntry> TimeEntry { get; }
    IUserProfileRepository UserProfile { get; }
    IRepository<Webhook> Webhook { get; }
    IRepository<RefreshToken> RefreshToken { get; }

    Task SaveAsync(CancellationToken cancellationToken = default);
}
