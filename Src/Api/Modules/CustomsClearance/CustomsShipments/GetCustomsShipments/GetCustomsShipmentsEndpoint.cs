using BuildingBlocks.Application;
using CustomsClearance.Application.CustomsShipments.GetCustomsShipments;
using CustomsClearance.Domain.Enums;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;

namespace Api.Modules.CustomsClearance.CustomsShipments.GetCustomsShipments;

public static class GetCustomsShipmentsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/customs-clearance/shipments",
            async (
                ReportType reportType,
                [AsParameters] PaginationRequest pagination,
                ISender sender,
                CancellationToken ct,
                CustomsShipmentStatus status = CustomsShipmentStatus.Open) =>
            {
                var query = new GetCustomsShipmentsQuery(
                    reportType,
                    status,
                    pagination);

                var result = await sender.Send(query, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            }).RequirePermission(PermissionCatalog.CustomsShipmentsView);
    }
}
