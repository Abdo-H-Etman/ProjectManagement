namespace Domain.Entities.Models;

public class TimeEntry : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid TaskId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public decimal? Hours { get; set; }
    public string? Description { get; set; }
    public bool IsBillable { get; set; } = false;

    public ApplicationUser User { get; set; } = null!;
    public Task Task { get; set; } = null!;
}
