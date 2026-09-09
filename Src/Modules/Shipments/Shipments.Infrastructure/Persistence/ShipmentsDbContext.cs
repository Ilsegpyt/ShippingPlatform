using BuildingBlocks.Application;
using BuildingBlocks.Domain;
using BuildingBlocks.Domain.Outbox;
using Microsoft.EntityFrameworkCore;
using Shipments.Application.Abstractions;
using Shipments.Domain.Declarations;
using Shipments.Domain.Shipments;
using System.Text.Json;

namespace Shipments.Infrastructure.Persistence;

public sealed class ShipmentsDbContext : DbContext, IShipmentsUnitOfWork
{
    public ShipmentsDbContext(
        DbContextOptions<ShipmentsDbContext> options)
        : base(options)
    {
    }

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<Shipment> Shipments => Set<Shipment>();

    public DbSet<DeclarationFile> DeclarationFiles =>
        Set<DeclarationFile>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ShipmentsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
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
                    JsonSerializer.Serialize(
                        domainEvent,
                        domainEvent.GetType()),
                    domainEvent.OccurredOnUtc);

                OutboxMessages.Add(outboxMessage);
            }

            aggregate.ClearDomainEvents();
        }

        return await base.SaveChangesAsync(ct);
    }
}