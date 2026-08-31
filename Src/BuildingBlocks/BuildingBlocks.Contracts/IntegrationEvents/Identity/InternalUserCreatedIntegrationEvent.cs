using MediatR;

namespace BuildingBlocks.Contracts.IntegrationEvents.Identity;

public sealed record InternalUserCreatedIntegrationEvent(
    Guid UserId,
    string Name,
    string Email) : INotification;