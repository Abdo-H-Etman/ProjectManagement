using Application.DTOs.Attachment;
using Application.DTOs.Comment;
using Application.DTOs.Project;
using Application.DTOs.User;

namespace Application.DTOs.Task;

public record TaskDetailsDto : TaskDto
{
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? DeletedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public UserDto AssignedUser { get; init; } = null!;
    public UserDto CreatedBy { get; init; } = null!;
    public ProjectSummaryDto Project { get; init; } = null!;
    public TaskSummaryDto ParentTask { get; init; } = null!;
    public IEnumerable<TaskSummaryDto> SubTasks { get; init; } = null!;
    public IEnumerable<CommentDto> Comments { get; init; } = null!;
    public IEnumerable<AttachmentDto> Attachments { get; init; } = null!;
}
