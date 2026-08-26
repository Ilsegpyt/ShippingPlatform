using BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Persistence;

public static class DbContextDomainEventExtensions
{
    public static IReadOnlyList<IDomainEvent> GetDomainEvents(
        this DbContext context)
    {
        return context.ChangeTracker
            .Entries()
            .Select(x => x.Entity)
            .OfType<IAggregateRoot>()
            .SelectMany(x => x.DomainEvents)
            .ToList();
    }

    public static void ClearDomainEvents(this DbContext context)
    {
        foreach (var aggregate in context.ChangeTracker
            .Entries()
            .Select(x => x.Entity)
            .OfType<IAggregateRoot>())
        {
            aggregate.ClearDomainEvents();
        }
    }
}