using BuildingBlocks.Domain;

namespace Identity.Domain.Events;

public sealed record SubAccountCreatedEvent(Guid SubAccountId, Guid OrganizationId, DateTime OccurredOnUtc)
    : IDomainEvent;

public sealed record SubAccountStatusChangedEvent(
    Guid SubAccountId,
    SubAccountStatus NewStatus,
    DateTime OccurredOnUtc) : IDomainEvent;
