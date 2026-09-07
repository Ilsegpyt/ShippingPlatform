using BuildingBlocks.Domain;
using Customers.Domain.Entities;

namespace Customers.Domain.Events;

public sealed record CustomerStatusChangedEvent(Guid CustomerId, CustomerStatus NewStatus, DateTime OccurredOnUtc) : IDomainEvent;
