using BuildingBlocks.Domain;
using Shipments.Domain.Shipments;

namespace Shipments.Domain.Events;

public sealed record ShipmentStatusChangedDomainEvent(
    Guid ShipmentId,
    string ShipmentRef,
    Guid CustomerId,
    ShipmentStatus OldStatus,
    ShipmentStatus NewStatus,
    DateTime OccurredOnUtc) : IDomainEvent;