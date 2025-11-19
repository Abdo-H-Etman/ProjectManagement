using Domain.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class CheckListItemConfiguration : IEntityTypeConfiguration<CheckListItem>
{
    public void Configure(EntityTypeBuilder<CheckListItem> builder)
    {
        builder.ToTable("checklist_items");

        builder.Property(cli => cli.Title)
            .IsRequired()
            .HasMaxLength(500);
        builder.Property(cli => cli.Position)
            .HasDefaultValue(0);    

        builder.HasIndex(cli => cli.TaskId);
        builder.HasIndex(cli => new { cli.TaskId, cli.IsCompleted});
        builder.HasIndex(cli => new { cli.TaskId, cli.Position});

        builder.HasOne(cli => cli.Task)
               .WithMany(t => t.CheckListItems)
               .HasForeignKey(cli => cli.TaskId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
