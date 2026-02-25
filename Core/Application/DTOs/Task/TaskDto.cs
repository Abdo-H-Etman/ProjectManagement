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
    public DateTime? DueDate { get; init; }
    public bool IsDeleted { get; init; }

}
