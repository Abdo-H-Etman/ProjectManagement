using Domain.Entities.Enums;

namespace Domain.Entities.Models;

public class Attachment : BaseEntity
{
    public Guid? TaskId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid UploadedById { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public AttachmentType Type { get; set; } = AttachmentType.Other;

    public Task? Task { get; set; }
    public Project? Project { get; set; }
    public ApplicationUser UploadedBy { get; set; } = null!;
}
