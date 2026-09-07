using BuildingBlocks.Domain;

namespace Identity.Domain.Events;
// UnUsed

public sealed record SubAccountEmailChangedEvent(
    Guid SubAccountId,
    Guid UserId,
    string NewEmail,
    DateTime OccurredOnUtc) : IDomainEvent;