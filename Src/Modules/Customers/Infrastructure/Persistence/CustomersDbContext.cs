using BuildingBlocks.Domain;
using BuildingBlocks.Domain.Outbox;
using Customers.Application.Abstractions;
using Customers.Domain;
using Customers.Domain.SearchHistory;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Customers.Infrastructure.Persistence;

public sealed class CustomersDbContext : DbContext, ICustomersUnitOfWork
{
    public CustomersDbContext(
        DbContextOptions<CustomersDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<SearchHistory> SearchHistories => Set<SearchHistory>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(
            typeof(CustomersDbContext).Assembly);

        builder.Entity<OutboxMessage>()
            .ToTable("OutboxMessages", t => t.ExcludeFromMigrations());
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