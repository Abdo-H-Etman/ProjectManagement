using Domain.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntry>
{
    public void Configure(EntityTypeBuilder<TimeEntry> builder)
    {
        builder.ToTable("time_entries");

        builder.Property(te => te.Description).HasMaxLength(2000);

        builder.HasIndex(te => te.UserId);
        builder.HasIndex(te => te.TaskId);
        builder.HasIndex(te => new{ te.TaskId, te.IsBillable});
        builder.HasIndex(te => new{ te.UserId, te.StartTime});
        builder.HasIndex(te => te.EndTime)
               .HasFilter("[EndTime] IS NULL");

        builder.HasOne(te => te.Task)
               .WithMany(t => t.TimeEntries)
               .HasForeignKey(te => te.TaskId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
