

using MediatR;

namespace BuildingBlocks.Contracts.IntegrationEvents.Shipments;

public sealed record ShipmentStatusChangedIntegrationEvent(
    Guid ShipmentId,
    string ShipmentRef,
    Guid CustomerId,
    string OldStatus,
    string NewStatus,
    DateTime OccurredOnUtc) : INotification;