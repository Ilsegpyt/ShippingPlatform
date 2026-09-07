using BuildingBlocks.Domain;
using Identity.Domain.Enums;


namespace Identity.Domain.Events;

//Unused
public sealed record SubAccountStatusChangedEvent(
    Guid SubAccountId,
    SubAccountStatus NewStatus,
    DateTime OccurredOnUtc) : IDomainEvent;
