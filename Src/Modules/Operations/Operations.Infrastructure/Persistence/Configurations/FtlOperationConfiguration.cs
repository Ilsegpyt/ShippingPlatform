using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Configurations;

public sealed class FtlOperationConfiguration : IEntityTypeConfiguration<FtlOperation>
{
    public void Configure(EntityTypeBuilder<FtlOperation> builder)
    {
        builder.ToTable("FtlOperations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Ref)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Year)
            .IsRequired();

        builder.Property(x => x.Sequence)
            .IsRequired();

        builder.Property(x => x.OperationType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.ClientName)
            .HasMaxLength(200);

        builder.Property(x => x.BL)
            .HasMaxLength(120);

        builder.Property(x => x.BookingNumber)
            .HasMaxLength(120);

        builder.Property(x => x.ClearanceType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.CertificateNumber)
            .HasMaxLength(80);

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

        builder.Property(x => x.AOD)
            .HasMaxLength(50);

        builder.Property(x => x.CustomsArea)
            .HasMaxLength(200);

        builder.Property(x => x.Version)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(x => x.EnteredByUserId)
            .HasMaxLength(128);

        builder.Property(x => x.EnteredByName)
            .HasMaxLength(200);

        // Concurrency Lock
        builder.Property(x => x.LockedByName)
            .HasMaxLength(200);

        builder.Property(x => x.LockedAtUtc)
            .HasColumnType("datetime2");

        // Soft Delete
        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.DeletedAtUtc)
            .HasColumnType("datetime2");

        builder.Property(x => x.DeletedByName)
            .HasMaxLength(200);

        // Audit
        builder.Property(x => x.CreatedAtUtc)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(x => x.CreatedByUserId)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.CreatedByName)
            .IsRequired()
            .HasMaxLength(200);

        // Indexes
        builder.HasIndex(x => x.Ref)
            .IsUnique()
            .HasDatabaseName("UX_FtlOperations_Ref")
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(x => new { x.OperationType, x.IsDeleted, x.Status, x.CreatedAtUtc })
            .HasDatabaseName("IX_FtlOperations_Type_Deleted_Status_Created");

        builder.HasQueryFilter(x => !x.IsDeleted);

        // Relationships
        builder.HasMany(x => x.Shipments)
            .WithOne(x => x.Operation)
            .HasForeignKey(x => x.FtlOperationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Trucks)
            .WithOne(x => x.Operation)
            .HasForeignKey(x => x.FtlOperationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Trips)
            .WithOne(x => x.Operation)
            .HasForeignKey(x => x.FtlOperationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.StatusAudits)
            .WithOne(x => x.Operation)
            .HasForeignKey(x => x.FtlOperationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

