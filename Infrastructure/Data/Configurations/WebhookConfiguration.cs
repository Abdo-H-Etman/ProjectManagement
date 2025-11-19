using Domain.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class WebhookConfiguration : IEntityTypeConfiguration<Webhook>
{
    public void Configure(EntityTypeBuilder<Webhook> builder)
    {
        builder.ToTable("webhooks");

        builder.Property(w => w.Url).IsRequired().HasMaxLength(500);
        builder.Property(w => w.Description).HasMaxLength(500);
        builder.Property(w => w.Name).HasMaxLength(100);
        builder.Property(w => w.Secret).HasMaxLength(200);
        builder.Property(w => w.Events).IsRequired();

        builder.HasIndex(w => w.ProjectId);
        builder.HasIndex(w => new { w.ProjectId, w.IsActive });
        builder.HasIndex(w => w.LastTriggeredAt);

        builder.HasOne(w => w.Project)
               .WithMany()
               .HasForeignKey(w => w.ProjectId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
