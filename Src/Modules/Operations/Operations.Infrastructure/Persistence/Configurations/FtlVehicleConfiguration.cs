using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Configurations;

public sealed class FtlVehicleConfiguration : IEntityTypeConfiguration<FtlVehicle>
{
    public void Configure(EntityTypeBuilder<FtlVehicle> builder)
    {
        builder.ToTable("FtlVehicles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PlateNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.TruckType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.DeletedAtUtc)
            .HasColumnType("datetime2");

        builder.Property(x => x.DeletedByName)
            .HasMaxLength(200);

        builder.HasIndex(x => x.PlateNumber)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasOne(x => x.Transporter)
            .WithMany()
            .HasForeignKey(x => x.FtlTransporterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

