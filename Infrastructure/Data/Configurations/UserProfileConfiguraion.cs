using Domain.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class UserProfileConfiguraion : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("user_profiles");

        builder.Property(up => up.AvatarUrl).HasMaxLength(255);
        builder.Property(up => up.Bio).HasMaxLength(1000);
        builder.Property(up => up.PhoneNumber).HasMaxLength(13);
        builder.Property(up => up.JobTitle).HasMaxLength(100);
        builder.Property(up => up.Department).HasMaxLength(100);
        builder.Property(up => up.Location).HasMaxLength(100);

        builder.HasIndex(up => up.UserId).IsUnique();
    }
}
