using MediatR;

namespace BuildingBlocks.Contracts.IntegrationEvents.Identity;

public sealed record SubAccountCreatedIntegrationEvent(
     Guid SubAccountId,
    Guid UserId,
    string Name,
    string Email) : INotification;