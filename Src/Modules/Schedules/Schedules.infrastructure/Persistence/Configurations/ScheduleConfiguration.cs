using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Schedules.Domain.Schedule;

namespace Schedules.Infrastructure.Persistence.Configurations;

public sealed class ScheduleConfiguration
    : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.ToTable("Schedules");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RouteId)
            .HasMaxLength(100);

        builder.Property(x => x.Mode)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.DepartureDate)
            .IsRequired();

        builder.Property(x => x.Vessel)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Origin)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.DeparturePortCode)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.DepartureCountry)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Destination)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.ArrivalPortCode)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.ArrivalCountry)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Carrier)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.CarrierCode)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.VoyageNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Arrival)
            .IsRequired();

        builder.Property(x => x.TransitTime)
     .HasConversion<long>()
     .HasColumnType("bigint")
     .IsRequired();

        builder.Property(x => x.CutoffDate)
            .IsRequired();

        builder.Property(x => x.PortCutoffDate)
            .IsRequired();

        builder.Property(x => x.RateCurrency)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.ContainerSize)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.RateAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.RateRemarks)
            .HasMaxLength(500);

        builder.Property(x => x.ValidityDate)
            .IsRequired();

        builder.Property(x => x.FreeTimeAtPOD)
            .IsRequired();

        builder.Property(x => x.FreeTimeAtPOL)
            .IsRequired();

        builder.Property(x => x.TransshipmentData)
            .HasMaxLength(1000);

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);
    }
}
