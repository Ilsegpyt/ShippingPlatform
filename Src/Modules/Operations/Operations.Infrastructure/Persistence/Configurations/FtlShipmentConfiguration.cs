using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operations.Domain.Entities.Ftl;

namespace Operations.Infrastructure.Persistence.Configurations;

public sealed class FtlShipmentConfiguration : IEntityTypeConfiguration<FtlShipment>
{
    public void Configure(EntityTypeBuilder<FtlShipment> builder)
    {
        builder.ToTable("FtlShipments");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Ordinal)
            .IsRequired();

        builder.Property(x => x.ShipmentNumber)
            .HasMaxLength(100);

        builder.Property(x => x.MAWB)
            .HasMaxLength(100);

        builder.Property(x => x.HAWB)
            .HasMaxLength(100);

        builder.Property(x => x.CertificateNumber)
            .HasMaxLength(100);

        builder.Property(x => x.ClearanceType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.ClientName)
            .HasMaxLength(200);

        builder.Property(x => x.CustomsArea)
            .HasMaxLength(200);

        builder.Property(x => x.GrossWeight)
            .HasPrecision(18, 3);

        builder.Property(x => x.Cbm)
            .HasPrecision(10, 3);

        builder.Property(x => x.Commodity)
            .HasMaxLength(300);

        builder.Property(x => x.Terminal)
            .HasMaxLength(200);

        builder.Property(x => x.Destination)
            .HasMaxLength(200);

        builder.Property(x => x.FullAddress)
            .HasMaxLength(500);

        builder.Property(x => x.AOD)
            .HasMaxLength(50);

        builder.Property(x => x.TransporterInvoiceNo)
            .HasMaxLength(100);

        builder.Property(x => x.MissingDocs)
            .HasMaxLength(500);

        // Foreign Key
        builder.HasOne(x => x.Operation)
            .WithMany(x => x.Shipments)
            .HasForeignKey(x => x.FtlOperationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Fast search indexes (Non-unique, FCL architecture)
        builder.HasIndex(x => x.ShipmentNumber)
            .HasDatabaseName("IX_FtlShipments_ShipmentNumber")
            .HasFilter("[ShipmentNumber] IS NOT NULL");

        builder.HasIndex(x => x.HAWB)
            .HasDatabaseName("IX_FtlShipments_HAWB")
            .HasFilter("[HAWB] IS NOT NULL");

        builder.HasIndex(x => new { x.MAWB, x.HAWB })
            .HasDatabaseName("IX_FtlShipments_MAWB_HAWB")
            .HasFilter("[MAWB] IS NOT NULL AND [HAWB] IS NOT NULL");

        builder.HasIndex(x => new { x.CertificateNumber, x.ShipmentNumber })
            .HasDatabaseName("IX_FtlShipments_Cert_ShipmentNumber")
            .HasFilter("[CertificateNumber] IS NOT NULL AND [ShipmentNumber] IS NOT NULL");

        builder.HasIndex(x => new { x.FtlOperationId, x.Ordinal })
            .HasDatabaseName("IX_FtlShipments_Operation_Ordinal");
    }
}
