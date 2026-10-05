using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Configurations;

public sealed class OperationContainerConfiguration
    : IEntityTypeConfiguration<OperationContainer>
{
    public void Configure(EntityTypeBuilder<OperationContainer> builder)
    {
        builder.HasKey(x => x.Id);

        // -------------------------
        // Properties
        // -------------------------

        builder.Property(x => x.Ordinal)
            .IsRequired();

        builder.Property(x => x.ContainerNumber)
            .HasMaxLength(11);

        builder.Property(x => x.MissingDocs)
            .HasMaxLength(1000);

        // -------------------------
        // Ordinal
        // -------------------------

        builder.ToTable("OperationContainers", table =>
        {
            table.HasCheckConstraint(
                "CK_OperationContainer_Ordinal",
                "[Ordinal] >= 0 AND [Ordinal] < 200");
        });

        builder.HasIndex(x => new
        {
            x.OperationId,
            x.Ordinal
        })
        .IsUnique();

        // -------------------------
        // Legacy indexes
        // -------------------------

        builder.HasIndex(x => x.ContainerNumber)
            .HasFilter("[ContainerNumber] IS NOT NULL");

        builder.HasIndex(x => new
        {
            x.RequiredLoadingDate,
            x.ActualArrivalDate
        });

        // -------------------------
        // Relationships
        // -------------------------

        builder.HasOne(x => x.Operation)
            .WithMany(x => x.Containers)
            .HasForeignKey(x => x.OperationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Transporter)
            .WithMany()
            .HasForeignKey(x => x.TransporterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Driver)
            .WithMany()
            .HasForeignKey(x => x.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Vehicle)
            .WithMany()
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}