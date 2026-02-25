using Domain.Entities.Enums;

namespace Application.DTOs.Attachment;

public record CreateAttachmentDto
{
    public Guid? TaskId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? UploadedById { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string Type { get; set; } = AttachmentType.Other.ToString();
}
