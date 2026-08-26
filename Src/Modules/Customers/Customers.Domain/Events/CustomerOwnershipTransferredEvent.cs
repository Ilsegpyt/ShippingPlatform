using BuildingBlocks.Domain;

namespace Customers.Domain.Events;

public sealed record CustomerOwnershipTransferredEvent(
    Guid CustomerId, Guid PreviousOwnerUserId, Guid NewOwnerUserId, DateTime OccurredOnUtc) : IDomainEvent;