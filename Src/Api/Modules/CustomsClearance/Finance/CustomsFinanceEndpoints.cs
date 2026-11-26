using BuildingBlocks.Application;
using CustomsClearance.Application.CustomsShipments.Finance;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;

namespace Api.Modules.CustomsClearance.Finance;

// Customs Finance screen (Open Ocean Import shipments, Cairo dates).
// Reads need View; Approve / Exclude / Restore change data, so they need Edit.
public static class CustomsFinanceEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        // tab = Forecast | Request | Approved | Excluded; paged (pageNumber, pageSize <= 100, search).
        app.MapGet(
            "/api/customs-clearance/finance",
            async (
                [AsParameters] PaginationRequest pagination,
                ISender sender,
                CancellationToken ct,
                CustomsFinanceTab tab = CustomsFinanceTab.Forecast) =>
            {
                var result = await sender.Send(new GetCustomsFinanceQuery(tab, pagination), ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            }).RequirePermission(PermissionCatalog.CustomsShipmentsView);

        app.MapGet(
            "/api/customs-clearance/finance/counts",
            async (ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetCustomsFinanceCountsQuery(), ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            }).RequirePermission(PermissionCatalog.CustomsShipmentsView);

        app.MapPost(
            "/api/customs-clearance/finance/{id:guid}/approve",
            async (Guid id, ApproveCustomsFinanceRequest? request, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new ApproveCustomsFinanceCommand(id, request?.Note), ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.BadRequest(result.Error);
            }).RequirePermission(PermissionCatalog.CustomsShipmentsEdit);

        app.MapPost(
            "/api/customs-clearance/finance/{id:guid}/exclude",
            async (Guid id, ExcludeCustomsFinanceRequest request, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(
                    new ExcludeCustomsFinanceCommand(id, request.Mode, request.DeferredUntilDate, request.Note),
                    ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.BadRequest(result.Error);
            }).RequirePermission(PermissionCatalog.CustomsShipmentsEdit);

        app.MapPost(
            "/api/customs-clearance/finance/{id:guid}/restore",
            async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new RestoreCustomsFinanceCommand(id), ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.BadRequest(result.Error);
            }).RequirePermission(PermissionCatalog.CustomsShipmentsEdit);
    }
}

public sealed record ApproveCustomsFinanceRequest(string? Note);

public sealed record ExcludeCustomsFinanceRequest(
    CustomsFinanceExcludeMode Mode,
    DateOnly? DeferredUntilDate,
    string? Note);
