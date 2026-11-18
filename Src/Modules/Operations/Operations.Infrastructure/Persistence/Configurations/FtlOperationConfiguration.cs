using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operations.Domain.Entities.Ftl;

namespace Operations.Infrastructure.Persistence.Configurations;

public sealed class FtlOperationConfiguration : IEntityTypeConfiguration<FtlOperation>
{
    public void Configure(EntityTypeBuilder<FtlOperation> builder)
    {
        builder.ToTable("FtlOperations");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Ignore(x => x.Ref);

        builder.Property(x => x.OperationType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.NumberOfShipments)
            .IsRequired()
            .HasDefaultValue(1);

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

        // Delivery Audit
        builder.Property(x => x.DeliveredAtUtc)
            .HasColumnType("datetime2");

        builder.Property(x => x.DeliveredByUserId)
            .HasMaxLength(128);

        builder.Property(x => x.DeliveredByName)
            .HasMaxLength(200);

        // Indexes
        builder.HasIndex(x => new { x.OperationType, x.IsDeleted, x.Status, x.CreatedAtUtc })
            .HasDatabaseName("IX_FtlOperations_Type_Deleted_Status_Created");

        builder.HasIndex(x => new { x.Status, x.DeliveredAtUtc })
            .HasDatabaseName("IX_FtlOperations_Status_DeliveredAtUtc");

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
