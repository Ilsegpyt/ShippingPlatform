using MediatR;
using Tracking.Application.Tracking.GetShipmentTracking;

namespace Api.Modules.Tracking;

public static class GetShipmentTrackingEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/tracking/{shipmentId:guid}",
            async (
                Guid shipmentId,
                ISender sender,
                CancellationToken ct) =>
            {
                var result = await sender.Send(
                    new GetShipmentTrackingQuery(shipmentId),
                    ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            });
    }
}