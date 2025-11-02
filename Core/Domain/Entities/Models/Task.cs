using Domain.Entities.Enums;

namespace Domain.Entities.Models;

public class Task : BaseEntity
{
    public Guid ProjectId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.Pending;
    public DateTime? DueDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Guid? AssignedToId { get; set; }
    public Guid CreatedById { get; set; }
    public Guid? ParentTaskId { get; set; }
    public decimal? EstimatedHours { get; set; }
    public decimal? ActualHours { get; set; }

    public Project Project { get; set; } = null!;
    public ApplicationUser? AssignedTo { get; set; }
    public ApplicationUser CreatedBy { get; set; } = null!;
    public Task? ParentTask { get; set; }
    public ICollection<Task> SubTasks { get; set; } = [];
    public ICollection<Comment> Comments { get; set; } = [];
    public ICollection<Attachment> Attachments { get; set; } = [];
    public ICollection<TaskTag> TaskTags { get; set; } = [];
    public ICollection<TimeEntry> TimeEntries { get; set; } = [];
    public ICollection<CheckListItem> CheckListItems { get; set; } = [];
    public ICollection<TaskDependency> Dependencies { get; set; } = [];
    public ICollection<TaskDependency> Dependents { get; set; } = [];
}
