using Domain.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.ToTable("attachments");

        builder.Property(a => a.FileName).HasMaxLength(255);
        builder.Property(a => a.FileUrl).IsRequired().HasMaxLength(500);
        builder.Property(a => a.ContentType).HasMaxLength(100);
        builder.Property(a => a.Type).HasConversion<string>();

        builder.HasIndex(a => a.TaskId);
        builder.HasIndex(a => a.ProjectId);
        builder.HasIndex(a => a.UploadedById);
        builder.HasIndex(a => a.Type);
    }

}
