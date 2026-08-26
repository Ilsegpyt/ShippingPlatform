using BuildingBlocks.Domain;

namespace Identity.Domain.Events;

public sealed record SubAccountEmailChangedEvent(
    Guid SubAccountId,
    Guid UserId,
    string NewEmail,
    DateTime OccurredOnUtc) : IDomainEvent;