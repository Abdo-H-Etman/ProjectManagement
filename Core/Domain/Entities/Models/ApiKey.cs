namespace Domain.Entities.Models;

public class ApiKey : BaseEntity
{
    public Guid UserId { get; set; }
    public required string Name { get; set; } = string.Empty;
    public required string KeyHash { get; set; } = string.Empty;
    public string? Prefix { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime ExpiresAt { get; set; }
    public DateTime LastUsedAt { get; set; }
    public string? Scopes { get; set; }

    public ApplicationUser User { get; set; } = null!;
}
