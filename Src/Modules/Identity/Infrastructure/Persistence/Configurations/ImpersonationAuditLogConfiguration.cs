using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public sealed class ImpersonationAuditLogConfiguration
    : IEntityTypeConfiguration<ImpersonationAuditLog>
{
    public void Configure(EntityTypeBuilder<ImpersonationAuditLog> builder)
    {
        builder.ToTable("ImpersonationAuditLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ImpersonatorUserId)
            .IsRequired();

        builder.Property(x => x.TargetCustomerUserId)
            .IsRequired();

        builder.Property(x => x.StartedAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.ImpersonatorUserId);

        builder.HasIndex(x => x.TargetCustomerUserId);
    }
}