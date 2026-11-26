using CustomsClearance.Application.CustomsShipments.Alerts;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;

namespace Api.Modules.CustomsClearance.CustomsShipments.GetCustomsAlertsSummary;

public static class GetCustomsAlertsSummaryEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/customs-clearance/alerts/summary",
            async (ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetCustomsAlertsSummaryQuery(), ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            }).RequirePermission(PermissionCatalog.CustomsShipmentsView);
    }
}
