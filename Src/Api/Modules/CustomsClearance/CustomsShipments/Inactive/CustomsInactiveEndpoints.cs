using CustomsClearance.Application.CustomsShipments.Inactive;
using CustomsClearance.Domain.Enums;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;

namespace Api.Modules.CustomsClearance.CustomsShipments.Inactive;

// Soft delete (Inactive archive) and the lightweight counts for tab badges.
// The paged Inactive listing is GET /api/customs-clearance/shipments?isDeleted=true.
public static class CustomsInactiveEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        // Moves an Open or Closed shipment to the read-only Inactive archive. No reactivate endpoint.
        app.MapDelete(
            "/api/customs-clearance/shipments/{id:guid}",
            async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new DeactivateCustomsShipmentCommand(id), ct);

                return result.IsSuccess
                    ? Results.Ok(new { success = true })
                    : Results.BadRequest(result.Error);
            }).RequirePermission(PermissionCatalog.CustomsShipmentsDelete);

        // { open, closed, inactive } x { oceanImport, oceanExport, domestic, total } from one grouped query.
        app.MapGet(
            "/api/customs-clearance/counts",
            async (ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetCustomsShipmentCountsQuery(), ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            }).RequirePermission(PermissionCatalog.CustomsShipmentsView);

        // Distinct "Inactive by" names for the archive filter (max 200).
        app.MapGet(
            "/api/customs-clearance/shipments/inactive-by",
            async (ISender sender, CancellationToken ct, ReportType? reportType = null) =>
            {
                var result = await sender.Send(new GetCustomsInactiveByNamesQuery(reportType), ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            }).RequirePermission(PermissionCatalog.CustomsShipmentsView);
    }
}
