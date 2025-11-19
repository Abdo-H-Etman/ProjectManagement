using Domain.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
{
    public void Configure(EntityTypeBuilder<ProjectMember> builder)
    {
        builder.ToTable("project_members");

        builder.HasKey(pm => new { pm.ProjectId, pm.UserId });

        builder.Property(pm => pm.Role).HasConversion<string>();

        builder.HasIndex(pm => pm.ProjectId);
        builder.HasIndex(pm => pm.UserId);
        builder.HasIndex(pm => pm.Role);

        builder.HasOne(pm => pm.InvitedBy)
            .WithMany()
            .HasForeignKey(pm => pm.InvitedById)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
