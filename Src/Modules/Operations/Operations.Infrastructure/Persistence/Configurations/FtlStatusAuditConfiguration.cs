using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Configurations;

public sealed class FtlStatusAuditConfiguration : IEntityTypeConfiguration<FtlStatusAudit>
{
    public void Configure(EntityTypeBuilder<FtlStatusAudit> builder)
    {
        builder.ToTable("FtlStatusAudits", table =>
        {
            table.HasCheckConstraint("CK_FtlStatusAudits_StatusChanged", "[BeforeStatus] <> [AfterStatus]");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();
        builder.Property(x => x.BeforeStatus).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.AfterStatus).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.UserId).IsRequired().HasMaxLength(128);
        builder.Property(x => x.AtUtc).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(x => new { x.FtlOperationId, x.AtUtc });
        builder.HasOne(x => x.Operation).WithMany(x => x.StatusAudits).HasForeignKey(x => x.FtlOperationId).OnDelete(DeleteBehavior.Cascade);
    }
}

