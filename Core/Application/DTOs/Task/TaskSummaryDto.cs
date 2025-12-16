namespace Application.DTOs.Task;

public record class TaskSummaryDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = null!;
    public string Status { get; init; } = null!;
}
