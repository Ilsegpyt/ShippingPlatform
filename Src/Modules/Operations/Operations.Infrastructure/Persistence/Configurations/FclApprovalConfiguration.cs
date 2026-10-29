using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Configurations;

public sealed class FclApprovalConfiguration : IEntityTypeConfiguration<FclApproval>
{
    public void Configure(EntityTypeBuilder<FclApproval> builder)
    {
        builder.ToTable("FclApprovals");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.ApprovalKey).IsRequired().HasMaxLength(300);
        builder.Property(x => x.ApprovedByUserId).IsRequired().HasMaxLength(128);
        builder.Property(x => x.ApprovedAtUtc).IsRequired();

        builder.Property(x => x.ImportContainerId).IsRequired();
        builder.Property(x => x.ExportOperationId).IsRequired(false);

        builder.HasIndex(x => x.ApprovalKey).IsUnique().HasDatabaseName("UX_FclApprovals_ApprovalKey");
        builder.HasIndex(x => x.ImportContainerId).IsUnique().HasDatabaseName("UX_FclApprovals_ImportContainerId");
        builder.HasIndex(x => x.ExportOperationId);

        builder.HasOne(x => x.ImportContainer).WithMany().HasForeignKey(x => x.ImportContainerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ExportOperation).WithMany().HasForeignKey(x => x.ExportOperationId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
    }
}
