using MediatR;

namespace BuildingBlocks.Contracts.IntegrationEvents.Customers;

public sealed record CustomerVoiceCreatedIntegrationEvent(
    Guid CustomerVoiceId,
    Guid CustomerId,
    Guid ShipmentId,
    Guid UserId,
    string Subject,
    DateTime OccurredOnUtc) : INotification;