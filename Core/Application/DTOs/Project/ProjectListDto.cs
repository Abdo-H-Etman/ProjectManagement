using Application.DTOs.User;

namespace Application.DTOs;

public record ProjectListDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Visibility { get; init; } = string.Empty;
    public UserDto Owner { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public bool IsDeleted { get; init; }
    public DateTime? DeletedAt { get; init; }
    public bool IsArchived { get; init; }
}
