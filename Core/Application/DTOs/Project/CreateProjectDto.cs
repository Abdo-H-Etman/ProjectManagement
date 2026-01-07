using Domain.Entities.Enums;

namespace Application.DTOs.Project;

public record CreateProjectDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Visibility { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}
