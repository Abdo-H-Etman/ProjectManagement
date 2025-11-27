using Domain.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ProjectInvitationConfiguration : IEntityTypeConfiguration<ProjectInvitation>
{
    public void Configure(EntityTypeBuilder<ProjectInvitation> builder)
    {
       builder.ToTable("project_invitations");

       builder.HasKey(pi => pi.Id);

       builder.Property(pi => pi.Role)
              .IsRequired()
              .HasConversion<string>();
       builder.Property(pi => pi.Token)
              .HasMaxLength(500);

       builder.HasIndex(pi => pi.Token).IsUnique();
       builder.HasIndex(pi => pi.ProjectId);
       builder.HasIndex(pi => new { pi.Email, pi.ProjectId });
       builder.HasIndex(pi => new { pi.IsAccepted, pi.ExpiresAt });


       builder.HasOne(pi => pi.Project)
              .WithMany()
              .HasForeignKey(pi => pi.ProjectId)
              .OnDelete(DeleteBehavior.Cascade);

       builder.HasOne(pi => pi.InvitedBy)
              .WithMany()
              .HasForeignKey(pi => pi.InvitedById)
              .OnDelete(DeleteBehavior.Restrict);
    }
}
