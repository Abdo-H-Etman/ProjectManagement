namespace Domain.Entities.Models;

public class UserFavorite
{
    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser User { get; set; } = null!;
    public Project Project { get; set; } = null!;
}
