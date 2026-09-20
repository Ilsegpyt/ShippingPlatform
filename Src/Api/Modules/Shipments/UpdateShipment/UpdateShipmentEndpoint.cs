using Identity.Domain.ValueObjects;
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
            HttpContext httpContext,
            ISender sender,
            CancellationToken ct) =>
        {
            var userId = httpContext.User.GetUserId();

            var command = new UpdateShipmentCommand(
                shipmentId,
                userId,
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