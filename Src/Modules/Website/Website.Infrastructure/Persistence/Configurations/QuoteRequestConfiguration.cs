using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Website.Domain.Entities;

namespace Website.Infrastructure.Persistence.Configurations;

public class QuoteRequestConfiguration
    : IEntityTypeConfiguration<QuoteRequest>
{
    public void Configure(EntityTypeBuilder<QuoteRequest> builder)
    {
        builder.ToTable("QuoteRequests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Company)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Country)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.CountryCode)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.Phone)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Message)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(x => x.InterestType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.TransportModes)
            .HasConversion(
                modes => string.Join(",", modes),
                value => value
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Enum.Parse<Website.Domain.Enums.TransportMode>)
                    .ToList())
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.AnnualShipments)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();
    }
}