using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task = Domain.Entities.Models.Task;

namespace Infrastructure.Data.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<Task>
{
    public void Configure(EntityTypeBuilder<Task> builder)
    {
        builder.ToTable("tasks");

        builder.Property(t => t.Title).HasMaxLength(200);
        builder.Property(t => t.Description).HasMaxLength(5000);
        builder.Property(t => t.Status).HasConversion<string>();
        builder.Property(t => t.Priority).HasConversion<string>();
        builder.Property(t => t.EstimatedHours)
            .HasColumnType("decimal(18,2)");
        builder.Property(t => t.ActualHours)
            .HasColumnType("decimal(18,2)");

        builder.HasIndex(t => t.ProjectId);
        builder.HasIndex(t => t.AssignedToId);
        builder.HasIndex(t => t.ParentTaskId);
        builder.HasIndex(t => t.CreatedById);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.Priority);
        builder.HasIndex(t => t.DueDate);

        builder.HasOne(t => t.ParentTask)
            .WithMany(t => t.SubTasks)
            .HasForeignKey(t => t.ParentTaskId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(t => t.Attachments)
            .WithOne(a => a.Task)
            .HasForeignKey(a => a.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
