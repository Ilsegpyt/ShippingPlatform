using Identity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(x => x.Name).IsUnique();

        builder.OwnsMany(typeof(PermissionKey), "Permissions", permBuilder =>
        {
            permBuilder.ToTable("RolePermissions");
            permBuilder.WithOwner().HasForeignKey("RoleId");
            permBuilder.Property<int>("Id").ValueGeneratedOnAdd();
            permBuilder.HasKey("Id");
            permBuilder.Property("Value").HasColumnName("PermissionKey").HasMaxLength(100).IsRequired();
        });
        builder.Navigation("Permissions").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
