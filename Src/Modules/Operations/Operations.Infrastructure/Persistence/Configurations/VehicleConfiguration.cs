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
    
        builder.Property(x => x.IsDeleted)
            .IsRequired();

        builder.Property(x => x.DeletedAtUtc);

        builder.Property(x => x.DeletedByUserId);

        builder.Property(x => x.DeletedByName)
            .HasMaxLength(200);
}
}