
using BuildingBlocks.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Operations.Application.Abstractions;
using Operations.Domain.Entities.Common;
using Operations.Domain.Entities.Fcl;
using Operations.Domain.Entities.Ftl;

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
    public DbSet<FclApproval> FclApprovals => Set<FclApproval>();
    public DbSet<Destination> Destinations => Set<Destination>();
    public DbSet<OperationStatusAudit> OperationStatusAudits => Set<OperationStatusAudit>();

    // FTL Airport
    public DbSet<FtlOperation> FtlOperations => Set<FtlOperation>();
    public DbSet<FtlShipment> FtlShipments => Set<FtlShipment>();
    public DbSet<FtlTruck> FtlTrucks => Set<FtlTruck>();
    public DbSet<FtlOperationTrip> FtlOperationTrips => Set<FtlOperationTrip>();
    public DbSet<FtlTruckUpdate> FtlTruckUpdates => Set<FtlTruckUpdate>();
    public DbSet<FtlStatusAudit> FtlStatusAudits => Set<FtlStatusAudit>();
    public DbSet<FtlAirport> FtlAirports => Set<FtlAirport>();
    public DbSet<FtlTerminal> FtlTerminals => Set<FtlTerminal>();
    public DbSet<FtlTransporter> FtlTransporters => Set<FtlTransporter>();
    public DbSet<FtlDriver> FtlDrivers => Set<FtlDriver>();
    public DbSet<FtlVehicle> FtlVehicles => Set<FtlVehicle>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(
            typeof(OperationsDbContext).Assembly);

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                        v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime?, DateTime?>(
                        v => v.HasValue ? (v.Value.Kind == DateTimeKind.Utc ? v.Value : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)) : v,
                        v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v));
                }
            }
        }

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

    var ftlStatusChanges = ChangeTracker
        .Entries<FtlOperation>()
        .Where(entry =>
            entry.State == EntityState.Modified &&
            entry.Property(x => x.Status).IsModified &&
            entry.Property(x => x.Status).OriginalValue != entry.Entity.Status &&
            !FtlStatusAudits.Local.Any(a => a.FtlOperationId == entry.Entity.Id && a.AfterStatus == entry.Entity.Status))
        .Select(entry => new FtlStatusAudit(
            entry.Entity.Id,
            entry.Property(x => x.Status).OriginalValue,
            entry.Entity.Status,
            userId,
            now))
        .ToList();

    if (ftlStatusChanges.Count > 0)
        FtlStatusAudits.AddRange(ftlStatusChanges);

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
