using BuildingBlocks.Application;
using MediatR;
using Schedules.Contracts;
using Shipments.Contracts;

namespace Tracking.Application.Tracking.GetMyShipmentsTracking;

public sealed class GetMyShipmentsTrackingQueryHandler(
    IShipmentQueryService shipmentQueryService,
    IScheduleQueryService scheduleQueryService)
    : IRequestHandler<
        GetMyShipmentsTrackingQuery,
        Result<IReadOnlyList<ShipmentTrackingResponse>>>
{
    public async Task<Result<IReadOnlyList<ShipmentTrackingResponse>>> Handle(
        GetMyShipmentsTrackingQuery query,
        CancellationToken ct)
    {
        var shipments = await shipmentQueryService.GetByCustomerIdAsync(
            query.CustomerId,
            ct);

        var tracking = new List<ShipmentTrackingResponse>();

        foreach (var shipment in shipments)
        {
            var schedule = await scheduleQueryService.GetByIdAsync(
                shipment.ScheduleId,
                ct);

            if (schedule is null)
                continue;

            tracking.Add(
                new ShipmentTrackingResponse(
                    shipment.Id,
                    shipment.ShipmentRef,
                    shipment.Status,
                    schedule.Origin,
                    schedule.Destination,
                    schedule.DeparturePortCode,
                    schedule.ArrivalPortCode,
                    schedule.DepartureDate,
                    schedule.Arrival,
                    schedule.TransitTime));
        }

        return Result.Success<IReadOnlyList<ShipmentTrackingResponse>>(
            tracking);
    }
}