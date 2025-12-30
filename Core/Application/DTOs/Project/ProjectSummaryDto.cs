namespace Application.DTOs.Project;

public record class ProjectSummaryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string Status { get; init; } = null!;
    public bool IsArchived { get; init; }
    public bool IsDeleted { get; init; }
}
