using BuildingBlocks.Domain;

namespace Customers.Domain.Events;

public sealed record CustomerStatusChangedEvent(Guid CustomerId, CustomerStatus NewStatus, DateTime OccurredOnUtc) : IDomainEvent;
