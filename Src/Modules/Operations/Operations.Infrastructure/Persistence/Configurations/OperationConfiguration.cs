using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Configurations;

public sealed class OperationConfiguration : IEntityTypeConfiguration<Operation>
{
    public void Configure(EntityTypeBuilder<Operation> builder)
    {
        builder.HasKey(x => x.Id);

        // -------------------------
        // Properties
        // -------------------------

        builder.Property(x => x.ClientId)
            .IsRequired();

        builder.Property(x => x.ShippingLineId)
            .IsRequired();

        builder.Property(x => x.OperationType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.ShipmentNumber)
            .HasMaxLength(120);

        builder.Property(x => x.CertificateNumber)
            .HasMaxLength(80);

        builder.Property(x => x.DeliveredByUserId)
            .HasMaxLength(128);

        builder.Property(x => x.ReopenedByUserId)
            .HasMaxLength(128);

        builder.Property(x => x.ReopenReason)
            .HasMaxLength(500);

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

        // -------------------------
        // Soft Delete
        // -------------------------

        builder.Property(x => x.DeletedByName)
            .HasMaxLength(200);

        // -------------------------
        // Relationships
        // -------------------------

        builder.HasOne(x => x.ShippingLine)
            .WithMany()
            .HasForeignKey(x => x.ShippingLineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Containers)
            .WithOne(x => x.Operation)
            .HasForeignKey(x => x.OperationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ImportDetails)
            .WithOne(x => x.Operation)
            .HasForeignKey<ImportDetails>(x => x.OperationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ExportDetails)
            .WithOne(x => x.Operation)
            .HasForeignKey<ExportDetails>(x => x.OperationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}