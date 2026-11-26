
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operations.Domain.Entities;
using Operations.Domain.Entities.Fcl;

namespace Operations.Infrastructure.Persistence.Configurations;

public sealed class ExportDetailsConfiguration
    : IEntityTypeConfiguration<ExportDetails>
{
    public void Configure(EntityTypeBuilder<ExportDetails> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OperationId)
            .IsRequired();

        builder.Property(x => x.ClearanceType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.BookingNumber)
            .IsRequired()
            .HasMaxLength(120);

        builder.HasIndex(x => x.BookingNumber)
            .IsUnique()
            .HasDatabaseName("UX_ExportDetails_BookingNumber")
            .HasFilter("[BookingNumber] IS NOT NULL");

        builder.Property(x => x.POLId)
            .IsRequired();

        builder.Property(x => x.TerminalId)
            .IsRequired(false);

        builder.Property(x => x.PODId)
            .IsRequired(false);

        builder.Property(x => x.POWId)
            .IsRequired(false);

        builder.Property(x => x.CutOffDate)
            .HasColumnType("date");

        builder.Property(x => x.Customs)
            .HasMaxLength(160);

        

      

        builder.ToTable("ExportDetails", table =>
        {
            table.HasCheckConstraint(
                "CK_ExportDetails_ClearanceType",
                "[ClearanceType] IN ('Bosla', 'Cert')");
        });

        builder.HasOne(x => x.Operation)
            .WithOne(x => x.ExportDetails)
            .HasForeignKey<ExportDetails>(x => x.OperationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.POL)
            .WithMany()
            .HasForeignKey(x => x.POLId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Terminal)
            .WithMany()
            .HasForeignKey(x => x.TerminalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.TerminalId);

        builder.HasOne(x => x.POD)
            .WithMany()
            .HasForeignKey(x => x.PODId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.POW)
            .WithMany()
            .HasForeignKey(x => x.POWId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.OperationId)
            .IsUnique();
    }
}
