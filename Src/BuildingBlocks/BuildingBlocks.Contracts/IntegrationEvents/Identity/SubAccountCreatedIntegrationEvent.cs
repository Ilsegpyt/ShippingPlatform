using MediatR;

namespace BuildingBlocks.Contracts.IntegrationEvents.Identity;

public sealed record SubAccountCreatedIntegrationEvent(
    Guid SubAccountId,
    string Name,
    string Email) : INotification;