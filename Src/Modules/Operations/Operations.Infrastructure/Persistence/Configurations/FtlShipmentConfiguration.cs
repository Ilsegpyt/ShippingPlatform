using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Configurations;

public sealed class FtlShipmentConfiguration : IEntityTypeConfiguration<FtlShipment>
{
    public void Configure(EntityTypeBuilder<FtlShipment> builder)
    {
        builder.ToTable("FtlShipments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Ordinal)
            .IsRequired();

        builder.Property(x => x.ShipmentNumber)
            .HasMaxLength(120);

        builder.Property(x => x.ClientName)
            .HasMaxLength(200);

        builder.Property(x => x.HAWB)
            .HasMaxLength(120);

        builder.Property(x => x.CertificateNumber)
            .HasMaxLength(80);

        builder.Property(x => x.GrossWeight)
            .HasPrecision(18, 3);

        builder.Property(x => x.Cbm)
            .HasPrecision(10, 3);

        builder.Property(x => x.Commodity)
            .HasMaxLength(300);

        builder.Property(x => x.CustomsArea)
            .HasMaxLength(200);

        builder.Property(x => x.TransporterInvoiceNo)
            .HasMaxLength(100);

        builder.Property(x => x.MissingDocs)
            .HasMaxLength(1000);

        builder.HasIndex(x => new { x.FtlOperationId, x.Ordinal })
            .IsUnique()
            .HasDatabaseName("UX_FtlShipments_Operation_Ordinal");

        builder.HasOne(x => x.Operation)
            .WithMany(x => x.Shipments)
            .HasForeignKey(x => x.FtlOperationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

