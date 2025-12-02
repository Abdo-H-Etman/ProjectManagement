using Domain.Entities.Models;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CommentRepository : Repository<Comment>, ICommentRepository
{
    public CommentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Comment>> GetTaskCommentsAsync(Guid taskId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .AsNoTracking()
            .Where(c => c.TaskId == taskId)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Comment>> GetProjectCommentsAsync(Guid projectId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .AsNoTracking()
            .Where(c => c.ProjectId == projectId)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Comment>> GetCommentRepliesAsync(Guid parentCommentId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .AsNoTracking()
            .Where(c => c.ParentCommentId == parentCommentId)
            .ToListAsync(cancellationToken);

    public async Task<int> GetCommentCountAsync(Guid taskId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .AsNoTracking()
            .CountAsync(c => c.TaskId == taskId, cancellationToken);
}
