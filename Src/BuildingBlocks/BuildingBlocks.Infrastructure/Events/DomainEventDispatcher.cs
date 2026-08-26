using BuildingBlocks.Application.Events;
using BuildingBlocks.Domain;
using MediatR;

namespace BuildingBlocks.Infrastructure.Events;

public sealed class DomainEventDispatcher(IPublisher publisher)
    : IDomainEventDispatcher
{
    public async Task DispatchAsync(
        IEnumerable<IDomainEvent> events,
        CancellationToken ct = default)
    {
        foreach (var domainEvent in events)
        {
            await publisher.Publish(domainEvent, ct);
        }
    }
}