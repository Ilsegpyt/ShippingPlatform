
using Identity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class SubAccountConfiguration : IEntityTypeConfiguration<SubAccount>
{
    public void Configure(EntityTypeBuilder<SubAccount> builder)
    {
        builder.ToTable("SubAccounts");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrganizationId).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.Email).HasMaxLength(320).IsRequired();
        builder.Property(x => x.ScopeType).HasConversion<string>().HasMaxLength(20);




        // Additive scope list -> owned collection in its own table.
        builder.OwnsMany(typeof(PermissionScope), "Scopes", scopeBuilder =>
        {
            scopeBuilder.ToTable("SubAccountScopes");
            scopeBuilder.WithOwner().HasForeignKey("SubAccountId");
            scopeBuilder.Property<int>("Id").ValueGeneratedOnAdd();
            scopeBuilder.HasKey("Id");
            scopeBuilder.Property("Category").HasConversion<string>().HasMaxLength(20).HasColumnName("Category");
            scopeBuilder.Property("Service").HasConversion<string>().HasMaxLength(20).HasColumnName("Service");
            scopeBuilder.Property("Type").HasConversion<string>().HasMaxLength(20).HasColumnName("Type");
        });
        builder.Navigation("Scopes").UsePropertyAccessMode(PropertyAccessMode.Field);

        // Direct permission grants (only meaningful while GrantType = Custom).
        builder.OwnsMany(typeof(PermissionKey), "Permissions", permBuilder =>
        {
            permBuilder.ToTable("SubAccountPermissions");
            permBuilder.WithOwner().HasForeignKey("SubAccountId");
            permBuilder.Property<int>("Id").ValueGeneratedOnAdd();
            permBuilder.HasKey("Id");
            permBuilder.Property("Value").HasColumnName("PermissionKey").HasMaxLength(100).IsRequired();
        });
        builder.Navigation("Permissions").UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => x.OrganizationId);
        builder.HasIndex(x => x.UserId).IsUnique();
    }
}