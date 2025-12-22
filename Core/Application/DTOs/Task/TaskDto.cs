using Application.DTOs.Project;
using Application.DTOs.User;

namespace Application.DTOs.Task;

public record TaskDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Priority { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? DueDate { get; init; }
    public bool IsDeleted { get; init; }
    public DateTime? DeletedAt { get; init; }
    public UserDto AssignedUser { get; init; } = null!;
    public UserDto CreatedBy { get; init; } = null!;
    public ProjectSummaryDto Project { get; init; } = null!;
}
