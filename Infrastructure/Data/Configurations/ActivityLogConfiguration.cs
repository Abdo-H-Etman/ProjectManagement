using Domain.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
{
    public void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        builder.ToTable("activity_logs");

        builder.Property(al => al.Type)
            .HasConversion<string>()
            .IsRequired();
        builder.Property(al => al.Description)
            .HasMaxLength(1000);
        builder.Property(al => al.IpAddress)
            .HasMaxLength(50);
        builder.Property(al => al.UserAgent)
            .HasMaxLength(500);

        builder.HasIndex(al => al.UserId);
        builder.HasIndex(al => al.TaskId);
        builder.HasIndex(al => al.UserId);
        builder.HasIndex(al => new {al.UserId, al.CreatedAt});
        builder.HasIndex(al => new {al.ProjectId, al.CreatedAt});
        builder.HasIndex(al => new {al.Type, al.CreatedAt});
        builder.HasIndex(al => al.IpAddress);

        builder.HasOne(al => al.Task)
            .WithMany()
            .HasForeignKey(al => al.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
