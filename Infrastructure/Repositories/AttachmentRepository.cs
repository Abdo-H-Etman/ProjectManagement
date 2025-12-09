using System;
using Domain.Entities.Enums;
using Domain.Entities.Models;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AttachmentRepository : Repository<Attachment>, IAttachmentRepository
{
    public AttachmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Attachment>> GetTaskAttachmentsAsync(Guid taskId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .AsNoTracking()
            .Where(a => a.TaskId == taskId)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Attachment>> GetProjectAttachmentsAsync(Guid projectId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .AsNoTracking()
            .Where(a => a.ProjectId == projectId)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Attachment>> GetAttachmentsByTypeAsync(Guid taskId, AttachmentType type, CancellationToken cancellationToken = default) =>
        await _dbSet
            .AsNoTracking()
            .Where(a => a.TaskId == taskId && a.Type == type)
            .ToListAsync(cancellationToken);

    public async Task<long> GetTotalFileSizeAsync(Guid projectId, CancellationToken cancellationToken = default) =>
        await _dbSet
            .AsNoTracking()
            .Where(a => a.ProjectId == projectId)
            .SumAsync(a => a.FileSize, cancellationToken);
}
