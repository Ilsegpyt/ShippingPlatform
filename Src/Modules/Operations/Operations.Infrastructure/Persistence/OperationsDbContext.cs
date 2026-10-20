
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
    bool acceptAllChangesOnSuccess,
    CancellationToken cancellationToken = default)
{
    var userId = _currentUser.UserId.ToString();

    var now = DateTime.UtcNow;

    var addedOperations = ChangeTracker
        .Entries<Operation>()
        .Where(entry => entry.State == EntityState.Added)
        .ToList();

    if (addedOperations.Count > 0)
    {
        var userName = await _currentUser.GetUserNameAsync(
            cancellationToken);

        foreach (var entry in addedOperations)
        {
            entry.Entity.SetCreatedAudit(
                userId,
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
            userId,
            now))
        .ToList();

    if (statusChanges.Count > 0)
        OperationStatusAudits.AddRange(statusChanges);

    return await base.SaveChangesAsync(
        acceptAllChangesOnSuccess,
        cancellationToken);
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
