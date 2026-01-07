namespace Application.DTOs.Attachment;

public record AttachmentDto
{
    public Guid Id { get; init; }
    public string FileName { get; init; } = null!;
    public string FileUrl { get; init; } = null!;
    public DateTime UploadedAt { get; init; }
}
