using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Configurations;

public sealed class FtlTruckConfiguration : IEntityTypeConfiguration<FtlTruck>
{
    public void Configure(EntityTypeBuilder<FtlTruck> builder)
    {
        builder.ToTable("FtlTrucks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Ordinal)
            .IsRequired();

        builder.Property(x => x.TransporterName)
            .HasMaxLength(200);

        builder.Property(x => x.TruckType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.TruckPlateNumber)
            .HasMaxLength(100);

        builder.Property(x => x.DriverName)
            .HasMaxLength(200);

        builder.Property(x => x.DriverMobile)
            .HasMaxLength(50);

        builder.HasIndex(x => new { x.FtlOperationId, x.Ordinal })
            .IsUnique()
            .HasDatabaseName("UX_FtlTrucks_Operation_Ordinal");

        builder.HasOne(x => x.Operation)
            .WithMany(x => x.Trucks)
            .HasForeignKey(x => x.FtlOperationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Updates)
            .WithOne(x => x.Truck)
            .HasForeignKey(x => x.FtlTruckId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

