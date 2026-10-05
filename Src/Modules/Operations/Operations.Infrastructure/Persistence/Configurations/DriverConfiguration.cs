using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Configurations;

public sealed class DriverConfiguration
    : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DriverNumber)
            .IsRequired()
            .HasMaxLength(80);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.MobileNumber)
            .HasMaxLength(25);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasIndex(x => x.DriverNumber)
            .IsUnique();
    }
}