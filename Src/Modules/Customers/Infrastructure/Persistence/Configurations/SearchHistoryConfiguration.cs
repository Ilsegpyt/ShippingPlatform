using Customers.Domain;
using Customers.Domain.SearchHistory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Customers.Infrastructure.Persistence.Configurations;

public sealed class SearchHistoryConfiguration
    : IEntityTypeConfiguration<SearchHistory>
{
    public void Configure(
        EntityTypeBuilder<SearchHistory> builder)
    {
        builder.ToTable("SearchHistories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerId)
            .IsRequired();

        builder.Property(x => x.Origin)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Destination)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ContainerSize)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.DepartureDate)
            .IsRequired();

        builder.Property(x => x.RoutesFound)
            .IsRequired();

        builder.Property(x => x.SearchedOnUtc)
            .IsRequired();

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}