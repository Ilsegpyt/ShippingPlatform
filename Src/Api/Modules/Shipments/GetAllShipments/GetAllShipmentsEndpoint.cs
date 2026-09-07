using BuildingBlocks.Application;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Shipments.Application.Shipments.GetAllShipmentsQuery;

namespace Api.Modules.Shipments.GetAllShipments;

public static class GetAllShipmentsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/shipments", async (
            [AsParameters] PaginationRequest pagination,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new GetAllShipmentsQuery(pagination),
                ct);

            return Results.Ok(result);
        }).RequirePermission(PermissionCatalog.ShipmentsView);
        
    }
}