using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Configurations;

public sealed class ContainerUpdateConfiguration
    : IEntityTypeConfiguration<ContainerUpdate>
{
    public void Configure(EntityTypeBuilder<ContainerUpdate> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Details)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.CreatedByName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.CreatedByUserId)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasOne(x => x.OperationContainer)
            .WithMany(x => x.Updates)
            .HasForeignKey(x => x.OperationContainerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.OperationContainerId);
    }
}