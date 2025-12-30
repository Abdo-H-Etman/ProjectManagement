using Domain.Entities.Models;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Utilities;

public static class DependencyInjection
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IRepositoryManager, RepostoryManager>();
        services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IAttachmentRepository, AttachmentRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IRepository<RefreshToken>, Repository<RefreshToken>>();
        services.AddScoped<IRepository<ApiKey>, Repository<ApiKey>>();
        services.AddScoped<IRepository<CheckListItem>, Repository<CheckListItem>>();
        services.AddScoped<IRepository<ProjectInvitation>, Repository<ProjectInvitation>>();
        services.AddScoped<IRepository<Tag>, Repository<Tag>>();
        services.AddScoped<IRepository<TimeEntry>, Repository<TimeEntry>>();
        services.AddScoped<IRepository<UserProfile>, Repository<UserProfile>>();
    }
}
