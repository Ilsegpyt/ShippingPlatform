using BuildingBlocks.Domain;
using BuildingBlocks.Domain.Outbox;
using Identity.Application.Abstractions;
using Identity.Domain;
using Identity.Domain.Impersonation;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Text.Json;

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
    public DbSet<ImpersonationAuditLog> ImpersonationAuditLogs
        => Set<ImpersonationAuditLog>(); public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
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
    public override async Task<int> SaveChangesAsync(
    CancellationToken ct = default)
    {
        var aggregates = ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(x => x.Entity.DomainEvents.Count > 0)
            .Select(x => x.Entity)
            .ToList();

        foreach (var aggregate in aggregates)
        {
            foreach (var domainEvent in aggregate.DomainEvents)
            {
                var outboxMessage = new OutboxMessage(
                    Guid.NewGuid(),
                    domainEvent.GetType().AssemblyQualifiedName!,
                    JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
                    domainEvent.OccurredOnUtc);

                OutboxMessages.Add(outboxMessage);
            }

            aggregate.ClearDomainEvents();
        }

        return await base.SaveChangesAsync(ct);
    }
}