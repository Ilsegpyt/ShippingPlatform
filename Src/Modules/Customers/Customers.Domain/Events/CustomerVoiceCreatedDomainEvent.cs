using BuildingBlocks.Domain;

namespace Customers.Domain.Events;

public sealed record CustomerVoiceCreatedDomainEvent(
    Guid CustomerVoiceId,
    Guid CustomerId,
    Guid ShipmentId,
    Guid UserId,
    string Subject,
    DateTime OccurredOnUtc) : IDomainEvent;