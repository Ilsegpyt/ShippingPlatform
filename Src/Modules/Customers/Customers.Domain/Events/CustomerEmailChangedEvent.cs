using BuildingBlocks.Domain;

namespace Customers.Domain.Events;

public sealed record CustomerEmailChangedEvent(
    Guid CustomerId,
    Guid OwnerUserId,
    string NewEmail,
    DateTime OccurredOnUtc) : IDomainEvent;