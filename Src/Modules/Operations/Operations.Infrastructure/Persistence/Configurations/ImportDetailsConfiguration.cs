using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Configurations;

public sealed class ImportDetailsConfiguration
    : IEntityTypeConfiguration<ImportDetails>
{
    public void Configure(EntityTypeBuilder<ImportDetails> builder)
    {
        builder.HasKey(x => x.Id);

        // -------------------------
        // Properties
        // -------------------------

        builder.Property(x => x.OperationId)
            .IsRequired();

        builder.Property(x => x.MBLNumber)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(x => x.Destination)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.PODId)
            .IsRequired();

        builder.Property(x => x.FreeTimeTill)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(x => x.RequiredOffloadingDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(x => x.RequiredOffloadingTime)
            .IsRequired();

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
        // Relationship
        // -------------------------

        builder.HasOne(x => x.Operation)
            .WithOne(x => x.ImportDetails)
            .HasForeignKey<ImportDetails>(x => x.OperationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.POD)
            .WithMany()
            .HasForeignKey(x => x.PODId)
            .OnDelete(DeleteBehavior.Restrict);

        // One ImportDetails per Operation
        builder.HasIndex(x => x.OperationId)
            .IsUnique();
    }
}