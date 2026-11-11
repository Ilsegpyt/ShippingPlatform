using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Configurations;

public sealed class FtlOperationTripConfiguration : IEntityTypeConfiguration<FtlOperationTrip>
{
    public void Configure(EntityTypeBuilder<FtlOperationTrip> builder)
    {
        builder.ToTable("FtlOperationTrips");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ExternalId)
            .HasMaxLength(100);

        builder.Property(x => x.Ordinal)
            .IsRequired();

        builder.Property(x => x.LoadStatus)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.LoadedWeight)
            .HasPrecision(18, 3);

        builder.Property(x => x.LoadedCbm)
            .HasPrecision(10, 3);

        builder.HasIndex(x => new { x.FtlOperationId, x.Ordinal })
            .IsUnique()
            .HasDatabaseName("UX_FtlOperationTrips_Operation_Ordinal");

        builder.HasOne(x => x.Operation)
            .WithMany(x => x.Trips)
            .HasForeignKey(x => x.FtlOperationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Shipment)
            .WithMany()
            .HasForeignKey(x => x.FtlShipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Truck)
            .WithMany()
            .HasForeignKey(x => x.FtlTruckId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

