using CustomsClearance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomsClearance.Infrastructure.Persistence.Configurations;

public class CustomsUpdateLogConfiguration
    : IEntityTypeConfiguration<CustomsUpdateLog>
{
    public void Configure(EntityTypeBuilder<CustomsUpdateLog> builder)
    {
        builder.ToTable("CustomsUpdateLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UpdateText)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedBy)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(x => new { x.ShipmentId, x.UpdatedAtUtc });
    }
}
