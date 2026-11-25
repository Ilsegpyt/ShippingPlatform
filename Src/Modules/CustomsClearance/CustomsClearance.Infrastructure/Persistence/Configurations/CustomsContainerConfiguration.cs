using CustomsClearance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomsClearance.Infrastructure.Persistence.Configurations;

public class CustomsContainerConfiguration
    : IEntityTypeConfiguration<CustomsContainer>
{
    public void Configure(EntityTypeBuilder<CustomsContainer> builder)
    {
        builder.ToTable("CustomsContainers", table =>
        {
            table.HasCheckConstraint(
                "CK_CustomsContainers_Ordinal",
                "[Ordinal] >= 1");

            table.HasCheckConstraint(
                "CK_CustomsContainers_ContainerType",
                "[ContainerType] IN ('DryStandard20', 'DryStandard40', 'DryHigh40', 'DryHigh45', 'ReeferStandard20', 'ReeferHigh40', 'OpenTop20', 'OpenTop40', 'OpenTopHigh40', 'FlatStandard40', 'FlatHigh40', 'Flat20', 'Tank20', 'Tank40', '20', '40', '40 HC', '45 HC')");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Ordinal)
            .IsRequired();

        builder.Property(x => x.ContainerType)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(x => new { x.ShipmentId, x.Ordinal })
            .IsUnique();
    }
}
