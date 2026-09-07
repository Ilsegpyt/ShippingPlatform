using BuildingBlocks.Domain;

namespace BuildingBlocks.Application.Events;

// unUsed now

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct = default);

}