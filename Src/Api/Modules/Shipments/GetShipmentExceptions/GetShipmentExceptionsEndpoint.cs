using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Shipments.Application.Shipments.GetShipmentExceptions;

namespace Api.Modules.Shipments.GetShipmentExceptions;

public static class GetShipmentExceptionsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/shipments/exceptions", async (
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new GetShipmentExceptionsQuery(),
                ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        }).RequirePermission(PermissionCatalog.ShipmentsView);
    }
}