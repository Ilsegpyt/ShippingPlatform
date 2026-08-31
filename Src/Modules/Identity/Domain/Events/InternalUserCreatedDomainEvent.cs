using BuildingBlocks.Domain;

namespace Identity.Domain.Events;

public sealed record InternalUserCreatedDomainEvent(
    Guid UserId,
    string Name,
    string Email,
    DateTime OccurredOnUtc) : IDomainEvent;