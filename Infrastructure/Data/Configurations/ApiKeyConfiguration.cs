using Domain.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.ToTable("api_keys");

        builder.HasKey(ak => ak.Id);

        builder.Property(ak => ak.Name).HasMaxLength(100);
        builder.Property(ak => ak.KeyHash).HasMaxLength(1000);

        builder.HasIndex(ak => ak.UserId);
        builder.HasIndex(ak => ak.KeyHash).IsUnique();
        builder.HasIndex(ak => ak.Prefix);
        builder.HasIndex(ak => new { ak.UserId, ak.IsActive });
        builder.HasIndex(ak => ak.ExpiresAt);

        builder.HasOne(ak => ak.User)
               .WithMany()
               .HasForeignKey(ak => ak.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}