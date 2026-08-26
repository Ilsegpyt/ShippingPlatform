using Identity.Application.Abstractions;
using Identity.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Identity.Infrastructure.Persistence;

public sealed class IdentityDbContext
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>,
      IIdentityUnitOfWork
{
    private IDbContextTransaction? _transaction;

    public IdentityDbContext(
        DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    public DbSet<SubAccount> SubAccounts => Set<SubAccount>();
    public DbSet<InternalUser> InternalUsers => Set<InternalUser>();
    public DbSet<Role> BusinessRoles => Set<Role>();
    public DbSet<AccountManagerAssignment> AccountManagerAssignments => Set<AccountManagerAssignment>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<ImpersonationAuditLog> ImpersonationAuditLogs => Set<ImpersonationAuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(IdentityDbContext).Assembly);

        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<ApplicationRole>().ToTable("IdentityRoles");
    }

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        _transaction = await Database.BeginTransactionAsync(ct);
    }

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction is null)
            return;

        await _transaction.CommitAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction is null)
            return;

        await _transaction.RollbackAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }
}