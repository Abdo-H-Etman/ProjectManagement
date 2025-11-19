using Domain.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class TaskDependencyConfiguration : IEntityTypeConfiguration<TaskDependency>
{
    public void Configure(EntityTypeBuilder<TaskDependency> builder)
    {
        builder.ToTable("task_dependencies");

        builder.HasKey(td => new { td.TaskId, td.DependsOnTaskId });

        builder.HasIndex(td => td.DependsOnTaskId);

        builder.HasOne(td => td.Task)
               .WithMany(t => t.Dependencies)
               .HasForeignKey(td => td.TaskId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(td => td.DependsOnTask)
               .WithMany(t => t.Dependents)
               .HasForeignKey(td => td.DependsOnTaskId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
