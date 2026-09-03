using Identity.Domain;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shipments.Application.Shipments.DeleteShipments;

namespace Api.Modules.Shipments.DeleteShipments;

public static class DeleteShipmentsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete(
            "/api/shipments",
            async (
                [FromBody] DeleteShipmentsRequest request,
                ISender sender,
                CancellationToken ct) =>
            {
                var command = new DeleteShipmentsCommand(
                    request.ShipmentIds);

                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            })
            .RequirePermission(PermissionCatalog.ShipmentsDelete);
    }
}