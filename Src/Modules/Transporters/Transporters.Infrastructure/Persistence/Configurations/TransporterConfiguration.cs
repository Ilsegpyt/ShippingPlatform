using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transporters.Domain.Entities;

namespace Transporters.Infrastructure.Persistence.Configurations;

public class TransporterConfiguration
    : IEntityTypeConfiguration<Transporter>
{
    public void Configure(EntityTypeBuilder<Transporter> builder)
    {
        builder.ToTable("Transporters");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.ContactPerson)
            .HasMaxLength(200);

        builder.Property(x => x.Phone)
            .HasMaxLength(30);

        builder.Property(x => x.Email)
            .HasMaxLength(200);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasMany(x => x.PortCoverages)
            .WithOne()
            .HasForeignKey(x => x.TransporterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.ClientExceptions)
            .WithOne()
            .HasForeignKey(x => x.TransporterId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
