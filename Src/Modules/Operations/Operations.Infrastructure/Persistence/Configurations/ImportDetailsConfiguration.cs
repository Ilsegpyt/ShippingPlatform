
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Configurations;

public sealed class ImportDetailsConfiguration
    : IEntityTypeConfiguration<ImportDetails>
{
    public void Configure(EntityTypeBuilder<ImportDetails> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OperationId)
            .IsRequired();

        builder.Property(x => x.MBLNumber)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(x => x.PODId)
            .IsRequired();

        builder.Property(x => x.RequiredOffloadingDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(x => x.RequiredOffloadingTime)
            .IsRequired();



        builder.HasOne(x => x.Operation)
            .WithOne(x => x.ImportDetails)
            .HasForeignKey<ImportDetails>(x => x.OperationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.POD)
            .WithMany()
            .HasForeignKey(x => x.PODId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.OperationId)
            .IsUnique();
    }
}
