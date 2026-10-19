
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Operations.Application.Abstractions;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence;

public sealed class OperationsDbContext : DbContext, IOperationsUnitOfWork
{
    private readonly ICurrentUser _currentUser;
    private IDbContextTransaction? _currentTransaction;

    public OperationsDbContext(
        DbContextOptions<OperationsDbContext> options,
        ICurrentUser currentUser)
        : base(options)
    {
        _currentUser = currentUser;
    }

    public DbSet<Operation> Operations => Set<Operation>();
    public DbSet<OperationContainer> OperationContainers => Set<OperationContainer>();
    public DbSet<ImportDetails> ImportDetails => Set<ImportDetails>();
    public DbSet<ExportDetails> ExportDetails => Set<ExportDetails>();
    public DbSet<ContainerUpdate> ContainerUpdates => Set<ContainerUpdate>();
    public DbSet<ShippingLine> ShippingLines => Set<ShippingLine>();
    public DbSet<Port> Ports => Set<Port>();
    public DbSet<Transporter> Transporters => Set<Transporter>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<FclSuggestion> FclSuggestions => Set<FclSuggestion>();
    public DbSet<FclApproval> FclApprovals => Set<FclApproval>();
    public DbSet<OperationStatusAudit> OperationStatusAudits => Set<OperationStatusAudit>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(
            typeof(OperationsDbContext).Assembly);

        builder.Entity<Operation>()
            .HasQueryFilter(x => !x.IsDeleted);

        builder.Entity<OperationContainer>()
            .HasQueryFilter(x => !x.IsDeleted);

        builder.Entity<ImportDetails>()
            .HasQueryFilter(x => !x.IsDeleted);

        builder.Entity<ExportDetails>()
            .HasQueryFilter(x => !x.IsDeleted);

        base.OnModelCreating(builder);
    }

    public async Task BeginTransactionAsync(
        CancellationToken ct = default)
    {
        if (_currentTransaction is not null)
        {
            throw new InvalidOperationException(
                "A transaction is already active.");
        }

        _currentTransaction =
            await Database.BeginTransactionAsync(ct);
    }

    public async Task CommitTransactionAsync(
        CancellationToken ct = default)
    {
        if (_currentTransaction is null)
        {
            throw new InvalidOperationException(
                "No active transaction exists.");
        }

        try
        {
            await _currentTransaction.CommitAsync(ct);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync(
        CancellationToken ct = default)
    {
        if (_currentTransaction is null)
            return;

        try
        {
            await _currentTransaction.RollbackAsync(ct);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUser.UserId;
        var userName = await _currentUser.GetUserNameAsync(
            cancellationToken);
        var now = DateTime.UtcNow;

        var deletedEntries = ChangeTracker
            .Entries()
            .Where(entry =>
                entry.State == EntityState.Deleted &&
                entry.Entity is ISoftDeletable)
            .ToList();

        foreach (var entry in deletedEntries)
        {
            var softDeletable = (ISoftDeletable)entry.Entity;

            softDeletable.MarkAsDeleted(
                userId,
                userName,
                now);

            entry.State = EntityState.Modified;
        }

        var auditEntries = ChangeTracker
            .Entries<AuditableEntity<Guid>>()
            .Where(entry =>
                entry.State == EntityState.Added ||
                (entry.State == EntityState.Modified &&
                 !deletedEntries.Contains(entry)))
            .ToList();

        foreach (var entry in auditEntries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.SetCreatedAudit(
                    userId.ToString(),
                    userName,
                    now);
            }
            else
            {
                entry.Entity.SetUpdatedAudit(
                    userId.ToString(),
                    userName,
                    now);
            }
        }

        var statusChanges = ChangeTracker
            .Entries<Operation>()
            .Where(entry =>
                entry.State == EntityState.Modified &&
                entry.Property(x => x.Status).IsModified &&
                entry.Property(x => x.Status).OriginalValue !=
                entry.Entity.Status)
            .Select(entry => new OperationStatusAudit(
                entry.Entity.Id,
                entry.Property(x => x.Status).OriginalValue,
                entry.Entity.Status,
                userId.ToString(),
                now))
            .ToList();

        if (statusChanges.Count > 0)
            OperationStatusAudits.AddRange(statusChanges);

        var containerUpdateEntries = ChangeTracker
            .Entries<ContainerUpdate>()
            .Where(entry => entry.State == EntityState.Added)
            .ToList();

        foreach (var entry in containerUpdateEntries)
        {
            entry.Entity.SetCreatedAudit(
                userId.ToString(),
                userName,
                now);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _currentTransaction?.Dispose();
        _currentTransaction = null;
        base.Dispose();
    }

    public override async ValueTask DisposeAsync()
    {
        if (_currentTransaction is not null)
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        await base.DisposeAsync();
    }
}
