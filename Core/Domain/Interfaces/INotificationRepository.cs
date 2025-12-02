using Domain.Entities.Models;
using Task = System.Threading.Tasks.Task;

namespace Domain.Interfaces;

public interface INotificationRepository : IRepository<Notification>
{
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId, int limit = 50, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetUnreadUserNotificationsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default);
    Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);
}
