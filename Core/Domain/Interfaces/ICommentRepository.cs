using Domain.Entities.Models;

namespace Domain.Interfaces;

public interface ICommentRepository : IRepository<Comment>
{
    Task<IEnumerable<Comment>> GetTaskCommentsAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Comment>> GetProjectCommentsAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Comment>> GetCommentRepliesAsync(Guid parentCommentId, CancellationToken cancellationToken = default);
    Task<int> GetCommentCountAsync(Guid taskId, CancellationToken cancellationToken = default);
}
