using Application.DTOs.User;

namespace Application.DTOs.Comment;

public record CommentDto
{
    public Guid Id { get; init; }
    public string Content { get; init; } = string.Empty;
    public bool IsEdited { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime EditedAt { get; init; }
    public UserDto Author { get; init; } = null!;
}
