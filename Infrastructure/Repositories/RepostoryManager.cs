using Domain.Entities.Models;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Task = System.Threading.Tasks.Task;

namespace Infrastructure.Repositories;

public class RepostoryManager : IRepositoryManager
{
    private readonly ApplicationDbContext _context;
    private IServiceProvider _serviceProvider;

    public RepostoryManager(ApplicationDbContext context, IServiceProvider serviceProvider)
    {
        _context = context;
        _serviceProvider = serviceProvider;
    }

    public IApplicationUserRepository User => _serviceProvider.GetRequiredService<IApplicationUserRepository>();
    public IProjectRepository Project => _serviceProvider.GetRequiredService<IProjectRepository>();
    public ITaskRepository Task => _serviceProvider.GetRequiredService<ITaskRepository>();
    public IActivityLogRepository ActivityLog => _serviceProvider.GetRequiredService<IActivityLogRepository>();
    public INotificationRepository Notification => _serviceProvider.GetRequiredService<INotificationRepository>();
    public IAttachmentRepository Attachment => _serviceProvider.GetRequiredService<IAttachmentRepository>();
    public ICommentRepository Comment => _serviceProvider.GetRequiredService<ICommentRepository>();
    public IRepository<ApiKey> ApiKey => _serviceProvider.GetRequiredService<IRepository<ApiKey>>();
    public IRepository<CheckListItem> CheckListItem => _serviceProvider.GetRequiredService<IRepository<CheckListItem>>();
    public IRepository<ProjectInvitation> ProjectInvitation => _serviceProvider.GetRequiredService<IRepository<ProjectInvitation>>();
    public IRepository<Tag> Tag => _serviceProvider.GetRequiredService<IRepository<Tag>>();
    public IRepository<TimeEntry> TimeEntry => _serviceProvider.GetRequiredService<IRepository<TimeEntry>>();
    public IRepository<UserProfile> UserProfile => _serviceProvider.GetRequiredService<IRepository<UserProfile>>();
    public IRepository<Webhook> Webhook => _serviceProvider.GetRequiredService<IRepository<Webhook>>();
    public async Task SaveAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);
    public void Dispose() => _context.Dispose();
    
}
