using Identity.Domain;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shipments.Application.Shipments.UpdateShipment;

namespace Api.Modules.Shipments.UpdateShipment;

public static class UpdateShipmentEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/shipments/{shipmentId:guid}", async (
            Guid shipmentId,
            [FromBody] UpdateShipmentRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateShipmentCommand(
                shipmentId,
                request.Status,
                request.MBL,
                request.HBL,
                request.MAWB,
                request.BookingConfirmationNumber);

            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.ShipmentsEdit);
    }
}