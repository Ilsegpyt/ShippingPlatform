using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transporters.Domain.Entities;

namespace Transporters.Infrastructure.Persistence.Configurations;

public class TransporterLaneRateConfiguration
    : IEntityTypeConfiguration<TransporterLaneRate>
{
    public void Configure(EntityTypeBuilder<TransporterLaneRate> builder)
    {
        builder.ToTable("TransporterLaneRates");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Destination)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.EquipmentType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);
    }
}
