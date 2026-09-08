using BuildingBlocks.Application;
using MediatR;
using Shipments.Contracts;
using Schedules.Contracts;

namespace Tracking.Application.Tracking.GetShipmentTracking;

public sealed class GetShipmentTrackingQueryHandler(
    IShipmentQueryService shipmentQueryService,
    IScheduleQueryService scheduleQueryService)
    : IRequestHandler<
        GetShipmentTrackingQuery,
        Result<ShipmentTrackingResponse>>
{
    public async Task<Result<ShipmentTrackingResponse>> Handle(
        GetShipmentTrackingQuery query,
        CancellationToken ct)
    {
        var shipment = await shipmentQueryService.GetByIdAsync(
            query.ShipmentId,
            ct);

        if (shipment is null)
        {
            return Result.Failure<ShipmentTrackingResponse>(
                "Shipment was not found.");
        }

        var schedule = await scheduleQueryService.GetByIdAsync(
            shipment.ScheduleId,
            ct);

        if (schedule is null)
        {
            return Result.Failure<ShipmentTrackingResponse>(
                "Schedule was not found.");
        }

        var response = new ShipmentTrackingResponse(
            shipment.Id,
            shipment.ShipmentRef,
            shipment.Status,
            schedule.Origin,
            schedule.Destination,
            schedule.DeparturePortCode,
            schedule.ArrivalPortCode,
            schedule.DepartureDate,
            schedule.Arrival,
            schedule.TransitTime);

        return Result.Success(response);
    }
}