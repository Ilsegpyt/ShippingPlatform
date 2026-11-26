using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operations.Domain.Entities.Fcl;

namespace Operations.Infrastructure.Persistence.Configurations;

public sealed class PortTerminalConfiguration : IEntityTypeConfiguration<PortTerminal>
{
    public void Configure(EntityTypeBuilder<PortTerminal> builder)
    {
        builder.ToTable("PortTerminals");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PortId)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Code)
            .HasMaxLength(40);

        builder.Property(x => x.ArabicName)
            .HasMaxLength(200);

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

        builder.HasOne(x => x.Port)
            .WithMany()
            .HasForeignKey(x => x.PortId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.PortId, x.Name })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(x => new { x.PortId, x.Code })
            .IsUnique()
            .HasFilter("[Code] IS NOT NULL AND [IsDeleted] = 0");

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
