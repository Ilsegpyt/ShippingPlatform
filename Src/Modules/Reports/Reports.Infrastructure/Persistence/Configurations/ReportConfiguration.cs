
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reports.Domain.Report;

namespace Reports.Infrastructure.Persistence.Configurations;

public sealed class ReportConfiguration
    : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerId)
            .IsRequired();

        builder.Property(x => x.ShipmentRef)
            .HasMaxLength(100);

        builder.Property(x => x.Category)
            .IsRequired();

        builder.Property(x => x.Service)
            .IsRequired();

        builder.Property(x => x.ShipmentType)
            .IsRequired();

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.StorageKey)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.UploadedByUserId)
            .IsRequired();

        builder.Property(x => x.UploadedAtUtc)
            .IsRequired();
    }
}
