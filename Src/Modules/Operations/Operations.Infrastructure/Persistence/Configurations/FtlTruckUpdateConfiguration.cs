using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Configurations;

public sealed class FtlTruckUpdateConfiguration : IEntityTypeConfiguration<FtlTruckUpdate>
{
    public void Configure(EntityTypeBuilder<FtlTruckUpdate> builder)
    {
        builder.ToTable("FtlTruckUpdates");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TripId)
            .HasMaxLength(100);

        builder.Property(x => x.Note)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(x => x.CreatedByUserId)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.CreatedByName)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(x => x.Truck)
            .WithMany(x => x.Updates)
            .HasForeignKey(x => x.FtlTruckId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

