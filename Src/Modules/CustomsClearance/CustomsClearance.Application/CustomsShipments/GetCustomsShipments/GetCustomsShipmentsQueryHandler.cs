using BuildingBlocks.Application;
using CustomsClearance.Application.Abstractions.Repositories;
using CustomsClearance.Application.CustomsShipments.Alerts;
using CustomsClearance.Domain.Enums;
using MediatR;

namespace CustomsClearance.Application.CustomsShipments.GetCustomsShipments;

public sealed class GetCustomsShipmentsQueryHandler(
    ICustomsShipmentRepository customsShipmentRepository,
    TimeProvider clock)
    : IRequestHandler<
        GetCustomsShipmentsQuery,
        Result<PagedResult<CustomsShipmentListItem>>>
{
    public async Task<Result<PagedResult<CustomsShipmentListItem>>> Handle(
        GetCustomsShipmentsQuery query,
        CancellationToken ct)
    {
        var pageNumber = query.Pagination.PageNumber;
        var pageSize = query.Pagination.PageSize;
        var search = query.Pagination.Search;
        var today = CustomsAlerts.CairoToday(clock);

        var alertsApply =
            query.ReportType == ReportType.OceanImport &&
            query.Status == CustomsShipmentStatus.Open;

        var filter = alertsApply && query.Alert is { } alert
            ? CustomsAlerts.Predicate(alert, today)
            : null;

        var (items, totalCount) =
            await customsShipmentRepository.GetPagedAsync(
                query.ReportType,
                query.Status,
                pageNumber,
                pageSize,
                search,
                filter,
                ct);

        var result = new PagedResult<CustomsShipmentListItem>(
            items
                .Select(x => new CustomsShipmentListItem(x, CustomsAlerts.Evaluate(x, today)))
                .ToList(),
            totalCount,
            pageNumber,
            pageSize);

        return Result.Success(result);
    }
}
