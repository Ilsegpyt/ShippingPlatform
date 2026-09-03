using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipments.Domain.Declarations;

namespace Shipments.Infrastructure.Persistence.Configurations;

public sealed class DeclarationFileConfiguration
    : IEntityTypeConfiguration<DeclarationFile>
{
    public void Configure(EntityTypeBuilder<DeclarationFile> builder)
    {
        builder.ToTable("DeclarationFiles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ShipmentId)
            .IsRequired();

        builder.Property(x => x.FileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.StorageKey)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.UploadedAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.ShipmentId);
    }
}