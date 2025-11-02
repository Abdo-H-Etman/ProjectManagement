using System;

namespace Domain.Entities.Models;

public class CheckListItem : BaseEntity
{
    public Guid TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; } = false;
    public DateTime? CompletedAt { get; set; }
    public int Position { get; set; }
    public Task Task { get; set; } = null!;
}
