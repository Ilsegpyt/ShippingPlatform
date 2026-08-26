using Customers.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Customers.Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.OwnerName).IsRequired().HasMaxLength(200);
        builder.Property(c => c.CompanyName).IsRequired().HasMaxLength(200);
        builder.Property(c => c.OwnerPhone).IsRequired().HasMaxLength(30);
        builder.Property(c => c.OwnerEmail).IsRequired().HasMaxLength(256);
        builder.Property(c => c.Industry).HasMaxLength(200);

        builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.OwnerUserId).IsRequired();

        builder.Property(c => c.IsDeleted).IsRequired();
        builder.Property(c => c.DeletedAtUtc);
        builder.Property(c => c.DeletedByUserId);

        builder.Ignore(c => c.DomainEvents);

        builder.HasIndex(c => c.OwnerUserId).IsUnique(); // Owner لشركة واحدة بس

        builder.HasQueryFilter(c => !c.IsDeleted); // Global filter — العملاء المحذوفين مش هيظهروا تلقائي
    }
}