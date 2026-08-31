
using BuildingBlocks.Domain;

namespace Customers.Domain.Events;

public sealed record CustomerRegisteredEvent(
    Guid CustomerId,
    Guid OwnerUserId,
    string OwnerName,
    string OwnerEmail,
    DateTime OccurredOnUtc) : IDomainEvent;