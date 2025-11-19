using Domain.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ProjectTagConfiguration : IEntityTypeConfiguration<ProjectTag>
{
    public void Configure(EntityTypeBuilder<ProjectTag> builder)
    {
        builder.ToTable("project_tags");

        builder.HasKey(pt => new { pt.ProjectId, pt.TagId });

        builder.HasIndex(pt => pt.TagId);
        builder.HasIndex(pt => pt.CreatedAt);

        builder.HasOne(pt => pt.Project)
               .WithMany(p => p.ProjectTags)
               .HasForeignKey(pt => pt.ProjectId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pt => pt.Tag)
               .WithMany(t => t.ProjectTags)
               .HasForeignKey(pt => pt.TagId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}