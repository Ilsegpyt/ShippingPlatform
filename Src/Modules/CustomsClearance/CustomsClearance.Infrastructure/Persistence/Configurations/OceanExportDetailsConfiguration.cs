using CustomsClearance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomsClearance.Infrastructure.Persistence.Configurations;

public class OceanExportDetailsConfiguration
    : IEntityTypeConfiguration<OceanExportDetails>
{
    public void Configure(EntityTypeBuilder<OceanExportDetails> builder)
    {
        builder.ToTable("OceanExportDetails", table =>
        {
            table.HasCheckConstraint(
                "CK_OceanExportDetails_ClearanceType",
                "[ClearanceType] IN ('Certificate', 'Bosla')");

            table.HasCheckConstraint(
                "CK_OceanExportDetails_CustomsArea",
                "[ClearanceType] = 'Bosla' OR ([ClearanceType] = 'Certificate' AND ([CustomsArea] IS NULL OR [CustomsArea] = ''))");

            table.HasCheckConstraint(
                "CK_OceanExportDetails_NumberOfContainers",
                "[NumberOfContainers] BETWEEN 1 AND 10000");

            table.HasCheckConstraint(
                "CK_OceanExportDetails_ContainerType",
                "[ContainerType] IS NULL OR [ContainerType] IN ('DryStandard20', 'DryStandard40', 'DryHigh40', 'DryHigh45', 'ReeferStandard20', 'ReeferHigh40', 'OpenTop20', 'OpenTop40', 'OpenTopHigh40', 'FlatStandard40', 'FlatHigh40', 'Flat20', 'Tank20', 'Tank40', '20', '40', '40 HC', '45 HC')");
        });

        builder.HasKey(x => x.ShipmentId);

        builder.Property(x => x.ClearanceType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.CustomsArea)
            .HasMaxLength(200);

        builder.Property(x => x.NumberOfContainers)
            .IsRequired();

        builder.Property(x => x.ContainerType)
            .HasMaxLength(30);

        builder.Property(x => x.Pol)
            .HasMaxLength(200);

        builder.Property(x => x.Pod)
            .HasMaxLength(200);

        builder.Property(x => x.BookingNumber)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(x => x.CommercialInvoiceNumber)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(x => x.Cutoff);

        builder.Property(x => x.LoadingDate)
            .HasColumnType("date");

        builder.Property(x => x.CertificateDate)
            .HasColumnType("date");

        builder.Property(x => x.DateOfCoo)
            .HasColumnType("date");

        builder.Property(x => x.DateOfReceivingForm13)
            .HasColumnType("date");

        builder.HasIndex(x => x.BookingNumber)
            .IsUnique()
            .HasDatabaseName("UX_OceanExportDetails_BookingNumber")
            .HasFilter("[BookingNumber] IS NOT NULL AND [BookingNumber] <> ''");

        builder.HasIndex(x => x.CommercialInvoiceNumber)
            .IsUnique()
            .HasDatabaseName("UX_OceanExportDetails_CommercialInvoiceNumber")
            .HasFilter("[CommercialInvoiceNumber] IS NOT NULL AND [CommercialInvoiceNumber] <> ''");
    }
}
