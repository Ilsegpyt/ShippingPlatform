using CustomsClearance.Application.CustomsShipments.GetCustomsShipmentById;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;

namespace Api.Modules.CustomsClearance.CustomsShipments.GetCustomsShipmentById;

public static class GetCustomsShipmentByIdEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/customs-clearance/shipments/{id:guid}",
            async (
                Guid id,
                ISender sender,
                CancellationToken ct) =>
            {
                var result = await sender.Send(
                    new GetCustomsShipmentByIdQuery(id),
                    ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.NotFound(result.Error);
            }).RequirePermission(PermissionCatalog.CustomsShipmentsView);
    }
}
