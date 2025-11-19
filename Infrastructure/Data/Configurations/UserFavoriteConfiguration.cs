using Domain.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class UserFavoriteConfiguration : IEntityTypeConfiguration<UserFavorite>
{
    public void Configure(EntityTypeBuilder<UserFavorite> builder)
    {
        builder.ToTable("user_favorites");

        builder.HasKey(uf => new { uf.UserId, uf.ProjectId });

        builder.HasIndex(uf => uf.ProjectId);
        builder.HasIndex(uf => uf.CreatedAt);
    }
}
