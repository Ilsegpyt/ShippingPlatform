using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Configurations;

public sealed class FtlDriverConfiguration : IEntityTypeConfiguration<FtlDriver>
{
    public void Configure(EntityTypeBuilder<FtlDriver> builder)
    {
        builder.ToTable("FtlDrivers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.DriverNumber)
            .HasMaxLength(80);

        builder.Property(x => x.MobileNumber)
            .HasMaxLength(50);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.DeletedAtUtc)
            .HasColumnType("datetime2");

        builder.Property(x => x.DeletedByName)
            .HasMaxLength(200);

        builder.HasIndex(x => x.DriverNumber)
            .IsUnique()
            .HasFilter("[DriverNumber] IS NOT NULL AND [IsDeleted] = 0");

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

