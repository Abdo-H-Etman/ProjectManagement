using Application.DTOs.Task;

namespace Application.DTOs.Project;

public record ProjectDetailsDto : ProjectListDto
{
    public IEnumerable<TaskSummaryDto> Tasks { get; init; } = [];
}
