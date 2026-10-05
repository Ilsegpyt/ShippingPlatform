using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore;
using Operations.Application.Abstractions;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence;

public sealed class OperationsDbContext : DbContext, IOperationsUnitOfWork
{
    private readonly ICurrentUser _currentUser;

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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(
            typeof(OperationsDbContext).Assembly);

        // -------------------------
        // Soft Delete Query Filters
        // -------------------------

        builder.Entity<Operation>()
            .HasQueryFilter(x => !x.IsDeleted);

        builder.Entity<OperationContainer>()
            .HasQueryFilter(x => !x.IsDeleted);

        builder.Entity<ImportDetails>()
            .HasQueryFilter(x => !x.IsDeleted);

        builder.Entity<ExportDetails>()
            .HasQueryFilter(x => !x.IsDeleted);

        builder.Entity<ShippingLine>()
            .HasQueryFilter(x => !x.IsDeleted);

        builder.Entity<Port>()
            .HasQueryFilter(x => !x.IsDeleted);

        builder.Entity<Transporter>()
            .HasQueryFilter(x => !x.IsDeleted);

        builder.Entity<Driver>()
            .HasQueryFilter(x => !x.IsDeleted);

        builder.Entity<Vehicle>()
            .HasQueryFilter(x => !x.IsDeleted);

        base.OnModelCreating(builder);
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUser.UserId;
        var userName = await _currentUser.GetUserNameAsync(cancellationToken);
        var now = DateTime.UtcNow;

        // -------------------------
        // Soft Delete
        // -------------------------

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

        // -------------------------
        // Audit
        // -------------------------

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

        // -------------------------
        // Container Update Audit
        // -------------------------

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
}