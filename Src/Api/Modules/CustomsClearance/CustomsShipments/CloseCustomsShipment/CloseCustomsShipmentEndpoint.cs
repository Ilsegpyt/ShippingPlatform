using CustomsClearance.Application.CustomsShipments.CloseCustomsShipment;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;

namespace Api.Modules.CustomsClearance.CustomsShipments.CloseCustomsShipment;

public static class CloseCustomsShipmentEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/api/customs-clearance/shipments/{id:guid}/close",
            async (
                Guid id,
                ISender sender,
                CancellationToken ct) =>
            {
                var result = await sender.Send(
                    new CloseCustomsShipmentCommand(id),
                    ct);

                return result.IsSuccess
                    ? Results.Ok()
                    : Results.BadRequest(result.Error);
            }).RequirePermission(PermissionCatalog.CustomsShipmentsClose);
    }
}
