using BuildingBlocks.Domain;

namespace BuildingBlocks.Application.Events;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(
        IEnumerable<IDomainEvent> events,
        CancellationToken ct = default);
}