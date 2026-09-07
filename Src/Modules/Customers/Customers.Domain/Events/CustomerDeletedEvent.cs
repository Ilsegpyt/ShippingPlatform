using BuildingBlocks.Domain;

namespace Customers.Domain.Events;

public sealed record CustomerDeletedEvent(
    Guid CustomerId,
    DateTime OccurredOnUtc) : IDomainEvent;