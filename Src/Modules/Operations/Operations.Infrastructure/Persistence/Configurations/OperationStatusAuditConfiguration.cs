using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Configurations;

public sealed class OperationStatusAuditConfiguration : IEntityTypeConfiguration<OperationStatusAudit>
{
    public void Configure(EntityTypeBuilder<OperationStatusAudit> builder)
    {
        builder.ToTable("audit_log", table =>
        {
            table.HasCheckConstraint("CK_audit_log_StatusChanged", "[BeforeStatus] <> [AfterStatus]");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();
        builder.Property(x => x.BeforeStatus).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.AfterStatus).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.UserId).IsRequired().HasMaxLength(128);
        builder.Property(x => x.AtUtc).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(x => new { x.OperationId, x.AtUtc });
        builder.HasOne(x => x.Operation).WithMany().HasForeignKey(x => x.OperationId).OnDelete(DeleteBehavior.Restrict);
    }
}
