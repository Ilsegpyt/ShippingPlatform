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
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var userId = httpContext.User.GetUserId();

            var tokenType =
                httpContext.User.FindFirst("token_type")?.Value;

            var organizationId =
                httpContext.User.FindFirst("org_id")?.Value;

            var query = new GetShipmentExceptionsQuery(
                userId,
                tokenType,
                organizationId);

            var result = await sender.Send(query, ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.ShipmentsView);
    }
}