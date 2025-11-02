namespace Domain.Entities.Models;

public class Tag : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
    public string? Description { get; set; }

    public ICollection<ProjectTag> ProjectTags { get; set; } = [];
    public ICollection<TaskTag> TaskTags { get; set; } = [];
}
