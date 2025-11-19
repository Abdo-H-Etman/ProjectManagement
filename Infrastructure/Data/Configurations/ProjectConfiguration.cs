using Domain.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");

        builder.Property(p => p.Name).HasMaxLength(200);
        builder.Property(p => p.Description).HasMaxLength(2000);
        builder.Property(p => p.Status).HasConversion<string>();
        builder.Property(p => p.Visibility).HasConversion<string>();
        
        builder.HasIndex(p => p.OwnerId);
        builder.HasIndex(p => p.Status);
        builder.HasIndex("IsArchived", "IsDeleted");
        builder.HasIndex(p => p.Name);

        builder.HasMany(p => p.Members)
            .WithOne(pm => pm.Project)
            .HasForeignKey(pm => pm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.Tasks)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.Attachments)
            .WithOne(a => a.Project)
            .HasForeignKey(a => a.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.ProjectTags)
            .WithOne(pt => pt.Project)
            .HasForeignKey(pt => pt.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.ActivityLogs)
            .WithOne(al => al.Project)
            .HasForeignKey(al => al.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.FavoritedBy)
            .WithOne(uf => uf.Project)
            .HasForeignKey(uf => uf.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);            
    }
}
