using CustomsClearance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomsClearance.Infrastructure.Persistence.Configurations;

public class CustomsShipmentConfiguration
    : IEntityTypeConfiguration<CustomsShipment>
{
    public void Configure(EntityTypeBuilder<CustomsShipment> builder)
    {
        builder.ToTable("CustomsShipments", table =>
        {
            table.HasCheckConstraint(
                "CK_CustomsShipments_ReportType",
                "[ReportType] IN ('OceanImport', 'OceanExport', 'Domestic')");

            table.HasCheckConstraint(
                "CK_CustomsShipments_Status",
                "[Status] IN ('Open', 'Closed')");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ReportType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.ShipmentNumber)
            .HasMaxLength(120);

        builder.Property(x => x.ClientId)
            .IsRequired();

        builder.Property(x => x.CertificateNumber)
            .HasMaxLength(120);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.LatestUpdate);

        builder.Property(x => x.ClosedAtUtc);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        // Soft delete (Inactive archive)
        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.DeletedAtUtc);

        builder.Property(x => x.DeletedByUserId)
            .HasMaxLength(128);

        builder.Property(x => x.DeletedByName)
            .HasMaxLength(200);

        // Every query sees active shipments only; the Inactive archive and by-id reads opt out with IgnoreQueryFilters().
        builder.HasQueryFilter(x => !x.IsDeleted);

        // Unique per report among active shipments (Open & Closed), blanks allowed; inactive ones free the number.
        builder.HasIndex(x => new { x.ReportType, x.ShipmentNumber })
            .IsUnique()
            .HasDatabaseName("UX_CustomsShipments_ReportType_ShipmentNumber")
            .HasFilter("[IsDeleted] = 0 AND [ShipmentNumber] IS NOT NULL AND [ShipmentNumber] <> ''");

        builder.HasIndex(x => new { x.ReportType, x.CertificateNumber })
            .IsUnique()
            .HasDatabaseName("UX_CustomsShipments_ReportType_CertificateNumber")
            .HasFilter("[IsDeleted] = 0 AND [CertificateNumber] IS NOT NULL AND [CertificateNumber] <> ''");

        // Active tables (Open / Closed per report).
        builder.HasIndex(x => new { x.IsDeleted, x.ReportType, x.Status })
            .HasDatabaseName("IX_CustomsShipments_IsDeleted_ReportType_Status");

        // Inactive archive, newest deactivated first; filtered so it never grows with active rows.
        builder.HasIndex(x => new { x.IsDeleted, x.ReportType, x.DeletedAtUtc })
            .HasDatabaseName("IX_CustomsShipments_Inactive_ReportType_DeletedAtUtc")
            .IsDescending(false, false, true)
            .HasFilter("[IsDeleted] = 1");

        builder.HasIndex(x => x.ClientId);

        builder.HasOne(x => x.OceanImportDetails)
            .WithOne()
            .HasForeignKey<OceanImportDetails>(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.OceanExportDetails)
            .WithOne()
            .HasForeignKey<OceanExportDetails>(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.DomesticImportDetails)
            .WithOne()
            .HasForeignKey<DomesticImportDetails>(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Containers)
            .WithOne()
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.UpdateLogs)
            .WithOne()
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
