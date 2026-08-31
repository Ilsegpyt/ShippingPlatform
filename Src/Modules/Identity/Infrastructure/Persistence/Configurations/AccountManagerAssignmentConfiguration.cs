using Identity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public sealed class AccountManagerAssignmentConfiguration
    : IEntityTypeConfiguration<AccountManagerAssignment>
{
    public void Configure(EntityTypeBuilder<AccountManagerAssignment> builder)
    {
        builder.ToTable("AccountManagerAssignments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AccountManagerId)
            .IsRequired();

        builder.Property(x => x.CustomerId)
            .IsRequired();

        // A Customer can have only one Account Manager.
        builder.HasIndex(x => x.CustomerId)
            .IsUnique();

        // Helps retrieve all Customers assigned to an Account Manager.
        builder.HasIndex(x => x.AccountManagerId);
    }
}