using CustomsClearance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomsClearance.Infrastructure.Persistence.Configurations;

public class DomesticImportDetailsConfiguration
    : IEntityTypeConfiguration<DomesticImportDetails>
{
    public void Configure(EntityTypeBuilder<DomesticImportDetails> builder)
    {
        builder.ToTable("DomesticImportDetails", table =>
        {
            table.HasCheckConstraint(
                "CK_DomesticImportDetails_NumberOfPieces",
                "[NumberOfPieces] >= 1");
        });

        builder.HasKey(x => x.ShipmentId);

        builder.Property(x => x.CommercialInvoiceNumber)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(x => x.NumberOfPieces)
            .IsRequired();

        builder.Property(x => x.MissingDocs)
            .HasMaxLength(2000);

        builder.Property(x => x.DateOfReceivingMissingDocs)
            .HasColumnType("date");

        builder.HasIndex(x => x.CommercialInvoiceNumber)
            .IsUnique()
            .HasDatabaseName("UX_DomesticImportDetails_CommercialInvoiceNumber")
            .HasFilter("[CommercialInvoiceNumber] IS NOT NULL AND [CommercialInvoiceNumber] <> ''");
    }
}
