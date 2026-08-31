using MediatR;

namespace BuildingBlocks.Contracts.IntegrationEvents.Identity;

public sealed record CustomerCreatedIntegrationEvent(
    Guid CustomerId,
    string Name,
    string Email) : INotification;