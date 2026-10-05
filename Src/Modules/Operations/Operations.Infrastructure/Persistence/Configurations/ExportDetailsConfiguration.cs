using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Configurations;

public sealed class ExportDetailsConfiguration
    : IEntityTypeConfiguration<ExportDetails>
{
    public void Configure(EntityTypeBuilder<ExportDetails> builder)
    {
        builder.HasKey(x => x.Id);

        // -------------------------
        // Properties
        // -------------------------

        builder.Property(x => x.OperationId)
            .IsRequired();

        builder.Property(x => x.ClearanceType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.BookingNumber)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(x => x.POLId)
            .IsRequired();

        builder.Property(x => x.POWId)
            .IsRequired(false);

        builder.Property(x => x.CutOffDate)
            .HasColumnType("date");

        builder.Property(x => x.Customs)
            .HasMaxLength(160);

        // -------------------------
        // Audit
        // -------------------------

        builder.Property(x => x.CreatedByUserId)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.CreatedByName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.UpdatedByUserId)
            .HasMaxLength(128);

        builder.Property(x => x.UpdatedByName)
            .HasMaxLength(200);

        builder.Property(x => x.DeletedByName)
            .HasMaxLength(200);

        // -------------------------
        // Constraints
        // -------------------------

        builder.ToTable("ExportDetails", table =>
        {
            table.HasCheckConstraint(
                "CK_ExportDetails_ClearanceType",
                "[ClearanceType] IN ('Bosla', 'Cert')");
        });

        // -------------------------
        // Relationships
        // -------------------------

        builder.HasOne(x => x.Operation)
            .WithOne(x => x.ExportDetails)
            .HasForeignKey<ExportDetails>(x => x.OperationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.POL)
            .WithMany()
            .HasForeignKey(x => x.POLId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.POW)
            .WithMany()
            .HasForeignKey(x => x.POWId)
            .OnDelete(DeleteBehavior.Restrict);

        // One ExportDetails per Operation
        builder.HasIndex(x => x.OperationId)
            .IsUnique();
    }
}