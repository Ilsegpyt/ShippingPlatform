using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Website.Domain.Entities;

namespace Website.Infrastructure.Persistence.Configurations;

public class BranchConfiguration
    : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("Branches");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Address)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.Latitude)
            .HasPrecision(10, 7)
            .IsRequired();

        builder.Property(x => x.Longitude)
            .HasPrecision(10, 7)
            .IsRequired();

        builder.Property(x => x.CeoName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.CeoEmail)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.GmName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.GmEmail)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();
    }
}