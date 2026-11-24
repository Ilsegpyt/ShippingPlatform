using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transporters.Domain.Entities;

namespace Transporters.Infrastructure.Persistence.Configurations;

public class TransporterClientExceptionConfiguration
    : IEntityTypeConfiguration<TransporterClientException>
{
    public void Configure(EntityTypeBuilder<TransporterClientException> builder)
    {
        builder.ToTable("TransporterClientExceptions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerId)
            .IsRequired();

        builder.Property(x => x.PortCoverageId)
            .IsRequired();

        builder.Property(x => x.Destination)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.EquipmentType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.AgreedPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.ValidFromUtc)
            .IsRequired();

        builder.Property(x => x.ValidToUtc)
            .IsRequired();

        builder.Property(x => x.Reason)
            .HasMaxLength(1000);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasIndex(x => x.CustomerId);
    }
}
