
using Identity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Configurations;

public sealed class AccountManagerAssignmentConfiguration : IEntityTypeConfiguration<AccountManagerAssignment>
{
    public void Configure(EntityTypeBuilder<AccountManagerAssignment> builder)
    {
        builder.ToTable("AccountManagerAssignments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.AccountManagerUserId).IsRequired();
        builder.Property(x => x.CustomerId).IsRequired();
        builder.HasIndex(x => new { x.AccountManagerUserId, x.CustomerId }).IsUnique();
    }
}

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TokenHash).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.TokenHash).IsUnique();
        builder.HasIndex(x => x.UserId);
    }
}

//public sealed class ImpersonationAuditLogConfiguration : IEntityTypeConfiguration<ImpersonationAuditLog>
//{
//    public void Configure(EntityTypeBuilder<ImpersonationAuditLog> builder)
//    {
//        builder.ToTable("ImpersonationAuditLogs");
//        builder.HasKey(x => x.Id);
//        builder.HasIndex(x => x.ImpersonatorUserId);
//    }
//}
