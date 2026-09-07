using BuildingBlocks.Domain;

namespace Customers.Domain.Events;

// to be done soon 

public sealed record CustomerOwnershipTransferredEvent(
    Guid CustomerId, Guid PreviousOwnerUserId, Guid NewOwnerUserId, DateTime OccurredOnUtc) : IDomainEvent;