using Domain.Entities.Enums;
using Domain.Entities.Models;

namespace Domain.Interfaces;

public interface IAttachmentRepository : IRepository<Attachment>
{
    Task<IEnumerable<Attachment>> GetTaskAttachmentsAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Attachment>> GetProjectAttachmentsAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Attachment>> GetAttachmentsByTypeAsync(Guid tasId, AttachmentType type, CancellationToken cancellationToken = default);
    Task<long> GetTotalFileSizeAsync(Guid projectId, CancellationToken cancellationToken = default);
}
