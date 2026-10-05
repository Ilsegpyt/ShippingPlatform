using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Configurations;

public sealed class VehicleConfiguration
    : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CarNumber)
            .IsRequired()
            .HasMaxLength(80);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasIndex(x => x.CarNumber)
            .IsUnique();
    }
}