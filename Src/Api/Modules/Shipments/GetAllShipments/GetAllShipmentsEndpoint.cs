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
            HttpContext httpContext,
            ISender sender,
            CancellationToken ct) =>
        {
            var userId = httpContext.User.GetUserId();

            var tokenType = httpContext.User.FindFirst("token_type")?.Value;

            var organizationId = httpContext.User.FindFirst("org_id")?.Value;

            var result = await sender.Send(
                new GetAllShipmentsQuery(
                    pagination,
                    userId,
                    tokenType,
                    organizationId),
                ct);

            return Results.Ok(result);
        })
        .RequirePermission(PermissionCatalog.ShipmentsView);
    }
}