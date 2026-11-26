using CustomsClearance.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomsClearance.Infrastructure.Persistence.Configurations;

public class CustomsFinanceRequestConfiguration
    : IEntityTypeConfiguration<CustomsFinanceRequest>
{
    public void Configure(EntityTypeBuilder<CustomsFinanceRequest> builder)
    {
        builder.ToTable("CustomsFinanceRequests", table =>
        {
            table.HasCheckConstraint(
                "CK_CustomsFinanceRequests_Status",
                "[Status] IN ('Pending', 'Approved', 'Deferred', 'OnHold')");

            table.HasCheckConstraint(
                "CK_CustomsFinanceRequests_DeferredUntilDate",
                "[Status] <> 'Deferred' OR [DeferredUntilDate] IS NOT NULL");
        });

        builder.HasKey(x => x.ShipmentId);

        builder.HasOne<CustomsShipment>()
            .WithOne()
            .HasForeignKey<CustomsFinanceRequest>(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.DeferredUntilDate)
            .HasColumnType("date");

        builder.Property(x => x.Note)
            .HasMaxLength(500);

        builder.Property(x => x.ApprovedByName)
            .HasMaxLength(200);

        builder.Property(x => x.UpdatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedByName)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(x => new { x.Status, x.DeferredUntilDate })
            .HasDatabaseName("IX_CustomsFinanceRequests_Status_DeferredUntilDate");
    }
}
