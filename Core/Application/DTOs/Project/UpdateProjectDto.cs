namespace Application.DTOs.Project;

public record UpdateProjectDto
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? Status { get; init; }
    public string? Visibility { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public bool? IsArchived { get; init; }
}
