using System.Text.Json;
using System.Text.Json.Nodes;
using BuildingBlocks.Application;
using CustomsClearance.Application.CustomsShipments.Alerts;
using CustomsClearance.Application.CustomsShipments.GetCustomsShipments;
using CustomsClearance.Application.CustomsShipments.Inactive;
using CustomsClearance.Domain.Enums;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.Extensions.Options;
using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

namespace Api.Modules.CustomsClearance.CustomsShipments.GetCustomsShipments;

public static class GetCustomsShipmentsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/customs-clearance/shipments",
            async (
                [AsParameters] PaginationRequest pagination,
                ISender sender,
                IOptions<HttpJsonOptions> jsonOptions,
                CancellationToken ct,
                ReportType? reportType = null,
                CustomsShipmentStatus status = CustomsShipmentStatus.Open,
                CustomsAlertFilter? alert = null,
                bool isDeleted = false,
                string? inactiveBy = null,
                CustomsInactiveDateType dateType = CustomsInactiveDateType.Inactive,
                DateOnly? dateFrom = null,
                DateOnly? dateTo = null) =>
            {
                var options = jsonOptions.Value.SerializerOptions;

                // Inactive (soft-deleted) archive: paged, newest deactivated first; reportType optional (all reports).
                if (isDeleted)
                {
                    var inactive = await sender.Send(
                        new GetInactiveCustomsShipmentsQuery(reportType, pagination, inactiveBy, dateType, dateFrom, dateTo),
                        ct);

                    if (!inactive.IsSuccess)
                        return Results.BadRequest(inactive.Error);

                    var p = inactive.Value;
                    var inactiveRows = p.Items
                        .Select(x =>
                        {
                            var row = JsonSerializer.SerializeToNode(x, options)!.AsObject();
                            row["alerts"] = null;
                            return row;
                        })
                        .ToList();

                    return Results.Ok(new PagedResult<JsonObject>(inactiveRows, p.TotalCount, p.PageNumber, p.PageSize));
                }

                if (reportType is null)
                    return Results.BadRequest("reportType is required.");

                var query = new GetCustomsShipmentsQuery(
                    reportType.Value,
                    status,
                    pagination,
                    alert);

                var result = await sender.Send(query, ct);

                if (!result.IsSuccess)
                    return Results.BadRequest(result.Error);

                // Same shipment JSON as before, plus an "alerts" field on every row.
                var page = result.Value;
                var items = page.Items
                    .Select(item =>
                    {
                        var row = JsonSerializer.SerializeToNode(item.Shipment, options)!.AsObject();
                        row["alerts"] = JsonSerializer.SerializeToNode(item.Alerts, options);
                        return row;
                    })
                    .ToList();

                return Results.Ok(new PagedResult<JsonObject>(
                    items,
                    page.TotalCount,
                    page.PageNumber,
                    page.PageSize));
            }).RequirePermission(PermissionCatalog.CustomsShipmentsView);
    }
}
