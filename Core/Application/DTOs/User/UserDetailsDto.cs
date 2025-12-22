using Application.DTOs.Project;

namespace Application.DTOs.User;

public record UserDetailsDto : UserDto
{
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? LastLoginAt { get; init; }
    public string TimeZone { get; init; } = null!;
    public bool IsActive { get; init; }
    public IEnumerable<ProjectSummaryDto> Projects { get; init; } = [];
}
