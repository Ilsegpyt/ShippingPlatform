using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transporters.Domain.Entities;

namespace Transporters.Infrastructure.Persistence.Configurations;

public class TransporterPortCoverageConfiguration
    : IEntityTypeConfiguration<TransporterPortCoverage>
{
    public void Configure(EntityTypeBuilder<TransporterPortCoverage> builder)
    {
        builder.ToTable("TransporterPortCoverages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PortName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.FreeHours)
            .IsRequired();

        builder.Property(x => x.DelayRatePerHour)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.OfficialReceiptNotes)
            .HasMaxLength(500);

        builder.Property(x => x.GeneralNotes)
            .HasMaxLength(2000);

        builder.Property(x => x.Status)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .IsRequired();

        builder.HasIndex(x => new { x.TransporterId, x.PortName })
            .IsUnique();

        builder.HasMany(x => x.LaneRates)
            .WithOne()
            .HasForeignKey(x => x.PortCoverageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.ClientExceptions)
            .WithOne()
            .HasForeignKey(x => x.PortCoverageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
