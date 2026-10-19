
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operations.Domain.Entities;
using Operations.Domain.Enums;

namespace Operations.Infrastructure.Persistence.Configurations;

public sealed class FclSuggestionConfiguration
    : IEntityTypeConfiguration<FclSuggestion>
{
    public void Configure(EntityTypeBuilder<FclSuggestion> builder)
    {
        builder.ToTable("FclSuggestions", table =>
        {
            table.HasCheckConstraint(
                "CK_FclSuggestions_MatchCount",
                "[MatchCount] >= 5 AND [MatchCount] <= 6");

            table.HasCheckConstraint(
                "CK_FclSuggestions_Status",
                "[Status] IN ('Pending', 'Approved')");

            table.HasCheckConstraint(
                "CK_FclSuggestions_ClearanceType",
                "[ClearanceType] IN ('Bosla', 'Cert')");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.ClientId).IsRequired();
        builder.Property(x => x.ClearanceType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.BookingNumber)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(x => x.ContainerNumber)
            .IsRequired()
            .HasMaxLength(11);

        builder.Property(x => x.ContainerType).IsRequired();

        // Nullable because a near match can be missing its destination.
        builder.Property(x => x.Destination)
            .HasMaxLength(200);

        builder.Property(x => x.RequiredLoadingDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(x => x.RequiredLoadingTime).IsRequired();
        builder.Property(x => x.CutOffDate).HasColumnType("date");

        builder.Property(x => x.FreeTimeMatch).IsRequired();
        builder.Property(x => x.ShippingLineMatch).IsRequired();
        builder.Property(x => x.PodPolMatch).IsRequired();
        builder.Property(x => x.OffloadingLoadingTimeMatch).IsRequired();
        builder.Property(x => x.ContainerSizeMatch).IsRequired();
        builder.Property(x => x.DestinationMatch).IsRequired();
        builder.Property(x => x.MatchCount).IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(FclSuggestionStatus.Pending);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(x => x.ImportOperationId);
        builder.HasIndex(x => x.ImportContainerId);
        builder.HasIndex(x => x.ExportOperationId);
        builder.HasIndex(x => x.ExportContainerId);
        builder.HasIndex(x => x.Status);

        builder.HasOne(x => x.ImportOperation)
            .WithMany()
            .HasForeignKey(x => x.ImportOperationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ImportContainer)
            .WithMany()
            .HasForeignKey(x => x.ImportContainerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ExportOperation)
            .WithMany()
            .HasForeignKey(x => x.ExportOperationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ExportContainer)
            .WithMany()
            .HasForeignKey(x => x.ExportContainerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ShippingLine)
            .WithMany()
            .HasForeignKey(x => x.ShippingLineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.POL)
            .WithMany()
            .HasForeignKey(x => x.POLId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.POW)
            .WithMany()
            .HasForeignKey(x => x.POWId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
