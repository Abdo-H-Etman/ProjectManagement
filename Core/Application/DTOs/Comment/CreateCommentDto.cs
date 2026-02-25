namespace Application.DTOs.Comment;

public record CreateCommentDto
{
    public Guid? TaskId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid AuthorId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public string Content { get; set; } = string.Empty;
}
