using CustomsClearance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomsClearance.Infrastructure.Persistence.Configurations;

public class OceanImportDetailsConfiguration
    : IEntityTypeConfiguration<OceanImportDetails>
{
    public void Configure(EntityTypeBuilder<OceanImportDetails> builder)
    {
        builder.ToTable("OceanImportDetails", table =>
        {
            table.HasCheckConstraint(
                "CK_OceanImportDetails_ClearanceType",
                "[ClearanceType] IN ('Import', 'Transit')");

            table.HasCheckConstraint(
                "CK_OceanImportDetails_AcidNumber",
                "[AcidNumber] IS NULL OR (LEN([AcidNumber]) = 19 AND [AcidNumber] NOT LIKE '%[^0-9]%')");

            table.HasCheckConstraint(
                "CK_OceanImportDetails_NumberOfContainers",
                "[NumberOfContainers] IS NULL OR [NumberOfContainers] BETWEEN 1 AND 10000");

            table.HasCheckConstraint(
                "CK_OceanImportDetails_FreeTime",
                "[FreeTime] IS NULL OR [FreeTime] >= 0");

            table.HasCheckConstraint(
                "CK_OceanImportDetails_ContainerType",
                "[ContainerType] IS NULL OR [ContainerType] IN ('DryStandard20', 'DryStandard40', 'DryHigh40', 'DryHigh45', 'ReeferStandard20', 'ReeferHigh40', 'OpenTop20', 'OpenTop40', 'OpenTopHigh40', 'FlatStandard40', 'FlatHigh40', 'Flat20', 'Tank20', 'Tank40', '20', '40', '40 HC', '45 HC')");
        });

        builder.HasKey(x => x.ShipmentId);

        builder.Property(x => x.ClearanceType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.AcidNumber)
            .HasMaxLength(19);

        builder.Property(x => x.Mbl)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(x => x.Hbl)
            .HasMaxLength(120);

        builder.Property(x => x.CustomsDeclarations)
            .HasMaxLength(500);

        builder.Property(x => x.ContainerType)
            .HasMaxLength(30);

        builder.Property(x => x.ShippingLine)
            .HasMaxLength(200);

        builder.Property(x => x.Commodity)
            .HasMaxLength(500);

        builder.Property(x => x.Pod)
            .HasMaxLength(200);

        builder.Property(x => x.Terminal)
            .HasMaxLength(200);

        builder.Property(x => x.EtaDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.AtaDate)
            .HasColumnType("date");

        builder.Property(x => x.DateOfFreeTimeEnd)
            .HasColumnType("date");

        builder.Property(x => x.DoDate)
            .HasColumnType("date");

        builder.Property(x => x.MissingDocs)
            .HasMaxLength(2000);

        builder.Property(x => x.InspectionDate)
            .HasColumnType("date");

        builder.Property(x => x.CurrentStage)
            .HasMaxLength(200);

        builder.HasIndex(x => x.AcidNumber)
            .IsUnique()
            .HasDatabaseName("UX_OceanImportDetails_AcidNumber")
            .HasFilter("[AcidNumber] IS NOT NULL AND [AcidNumber] <> ''");

        builder.HasIndex(x => x.Mbl)
            .IsUnique()
            .HasDatabaseName("UX_OceanImportDetails_Mbl")
            .HasFilter("[Mbl] IS NOT NULL AND [Mbl] <> ''");

        builder.HasIndex(x => x.Hbl)
            .IsUnique()
            .HasDatabaseName("UX_OceanImportDetails_Hbl")
            .HasFilter("[Hbl] IS NOT NULL AND [Hbl] <> ''");
    }
}
