using CustomsClearance.Application.CustomsShipments.GetCustomsUpdateLogs;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;

namespace Api.Modules.CustomsClearance.CustomsShipments.GetCustomsUpdateLogs;

public static class GetCustomsUpdateLogsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/customs-clearance/shipments/{id:guid}/updates",
            async (
                Guid id,
                ISender sender,
                CancellationToken ct) =>
            {
                var result = await sender.Send(
                    new GetCustomsUpdateLogsQuery(id),
                    ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.NotFound(result.Error);
            }).RequirePermission(PermissionCatalog.CustomsShipmentsView);
    }
}
