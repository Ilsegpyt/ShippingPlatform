using BuildingBlocks.Application;
using MediatR;

namespace Tracking.Application.Tracking.GetMyShipmentsTracking;

public sealed record GetMyShipmentsTrackingQuery(
    Guid CustomerId)
    : IRequest<Result<IReadOnlyList<ShipmentTrackingResponse>>>;