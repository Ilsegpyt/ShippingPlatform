using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipments.Domain.Shipments;

namespace Shipments.Infrastructure.Persistence.Configurations;

public sealed class ShipmentConfiguration
    : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.ToTable("Shipments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ShipmentRef)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.ShipmentRef)
            .IsUnique();

        builder.Property(x => x.CustomerId)
            .IsRequired();

        builder.Property(x => x.ScheduleId)
            .IsRequired();

        builder.Property(x => x.Mode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Carrier)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ContainerType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.Rate)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Total)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.MBL)
            .HasMaxLength(100);

        builder.Property(x => x.HBL)
            .HasMaxLength(100);

        builder.Property(x => x.MAWB)
            .HasMaxLength(100);

        builder.Property(x => x.BookingConfirmationNumber)
            .HasMaxLength(100);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.CustomerId);
        builder.HasIndex(x => x.ScheduleId);
        builder.HasIndex(x => x.Status);
    }
}