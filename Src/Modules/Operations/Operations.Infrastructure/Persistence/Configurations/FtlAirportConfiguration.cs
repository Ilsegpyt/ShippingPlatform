using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Configurations;

public sealed class FtlAirportConfiguration : IEntityTypeConfiguration<FtlAirport>
{
    public void Configure(EntityTypeBuilder<FtlAirport> builder)
    {
        builder.ToTable("FtlAirports");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Code)
            .HasMaxLength(50);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.DeletedAtUtc)
            .HasColumnType("datetime2");

        builder.Property(x => x.DeletedByName)
            .HasMaxLength(200);

        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

