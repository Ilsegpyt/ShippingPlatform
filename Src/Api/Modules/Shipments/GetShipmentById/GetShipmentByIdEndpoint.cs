using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Shipments.Application.Shipments.GetShipmentByIdQuery;

namespace Api.Modules.Shipments.GetShipmentById;

public static class GetShipmentByIdEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/shipments/{id:guid}", async (
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new GetShipmentByIdQuery(id),
                ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound(result.Error);
        })
        .RequirePermission(PermissionCatalog.ShipmentsView);
    }
}