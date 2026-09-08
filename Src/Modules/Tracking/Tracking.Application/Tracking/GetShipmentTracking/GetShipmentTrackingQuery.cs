using BuildingBlocks.Application;
using MediatR;

namespace Tracking.Application.Tracking.GetShipmentTracking;

public sealed record GetShipmentTrackingQuery(
    Guid ShipmentId)
    : IRequest<Result<ShipmentTrackingResponse>>;

