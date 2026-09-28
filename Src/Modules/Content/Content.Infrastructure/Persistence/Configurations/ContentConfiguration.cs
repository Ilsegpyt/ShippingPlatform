using ContentEntity = Content.Domain.Entities.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Content.Infrastructure.Persistence.Configurations;

public sealed class ContentConfiguration
    : IEntityTypeConfiguration<ContentEntity>
{
    public void Configure(
        EntityTypeBuilder<ContentEntity> builder)
    {
        builder.ToTable("Contents");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Body)
            .IsRequired(false);

        builder.Property(x => x.FeaturedImage)
            .IsRequired(false);

        builder.Property(x => x.LinkUrl)
            .IsRequired(false);

        builder.HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}