using BuildingBlocks.Application;
using CustomsClearance.Application.Abstractions.Repositories;
using CustomsClearance.Domain.Entities;
using CustomsClearance.Domain.Enums;
using MediatR;

namespace CustomsClearance.Application.CustomsShipments.Inactive;

// ---------- Paged Inactive archive ----------
// Dates are Cairo calendar days (inclusive); PageSize is clamped to 1..100 by PaginationRequest and here.
public sealed record GetInactiveCustomsShipmentsQuery(
    ReportType? ReportType,
    PaginationRequest Pagination,
    string? InactiveBy,
    CustomsInactiveDateType DateType,
    DateOnly? DateFrom,
    DateOnly? DateTo)
    : IRequest<Result<PagedResult<CustomsShipment>>>;

public sealed class GetInactiveCustomsShipmentsQueryHandler(
    ICustomsShipmentRepository customsShipmentRepository)
    : IRequestHandler<GetInactiveCustomsShipmentsQuery, Result<PagedResult<CustomsShipment>>>
{
    public async Task<Result<PagedResult<CustomsShipment>>> Handle(
        GetInactiveCustomsShipmentsQuery query,
        CancellationToken ct)
    {
        if (query.DateFrom is { } f && query.DateTo is { } t && f > t)
            return Result.Failure<PagedResult<CustomsShipment>>("The 'from' date must be on or before the 'to' date.");

        var pageNumber = Math.Max(1, query.Pagination.PageNumber);
        var pageSize = Math.Clamp(query.Pagination.PageSize, 1, 100);

        var filter = new CustomsInactiveFilter(
            query.ReportType,
            query.Pagination.Search,
            query.InactiveBy,
            query.DateType,
            query.DateFrom is { } from ? CustomsCairoTime.DayStartUtc(from) : null,
            query.DateTo is { } to ? CustomsCairoTime.DayStartUtc(to.AddDays(1)) : null);

        var (items, totalCount) = await customsShipmentRepository.GetInactivePagedAsync(
            filter, pageNumber, pageSize, ct);

        return Result.Success(new PagedResult<CustomsShipment>(items, totalCount, pageNumber, pageSize));
    }
}

// ---------- "Inactive by" filter options ----------
public sealed record GetCustomsInactiveByNamesQuery(ReportType? ReportType)
    : IRequest<Result<IReadOnlyList<string>>>;

public sealed class GetCustomsInactiveByNamesQueryHandler(
    ICustomsShipmentRepository customsShipmentRepository)
    : IRequestHandler<GetCustomsInactiveByNamesQuery, Result<IReadOnlyList<string>>>
{
    public async Task<Result<IReadOnlyList<string>>> Handle(
        GetCustomsInactiveByNamesQuery query,
        CancellationToken ct) =>
        Result.Success(await customsShipmentRepository.GetInactiveByNamesAsync(query.ReportType, ct));
}

// ---------- Counts for tab badges (one grouped query, no rows) ----------
public sealed record CustomsCountsByReport(int OceanImport, int OceanExport, int Domestic, int Total);

public sealed record CustomsShipmentCounts(
    CustomsCountsByReport Open,
    CustomsCountsByReport Closed,
    CustomsCountsByReport Inactive);

public sealed record GetCustomsShipmentCountsQuery : IRequest<Result<CustomsShipmentCounts>>;

public sealed class GetCustomsShipmentCountsQueryHandler(
    ICustomsShipmentRepository customsShipmentRepository)
    : IRequestHandler<GetCustomsShipmentCountsQuery, Result<CustomsShipmentCounts>>
{
    public async Task<Result<CustomsShipmentCounts>> Handle(
        GetCustomsShipmentCountsQuery query,
        CancellationToken ct)
    {
        var rows = await customsShipmentRepository.GetCountsAsync(ct);

        CustomsCountsByReport Sum(Func<CustomsShipmentCountRow, bool> pick)
        {
            int Of(ReportType r) => rows.Where(x => x.ReportType == r && pick(x)).Sum(x => x.Count);
            var oi = Of(ReportType.OceanImport);
            var oe = Of(ReportType.OceanExport);
            var d = Of(ReportType.Domestic);
            return new CustomsCountsByReport(oi, oe, d, oi + oe + d);
        }

        return Result.Success(new CustomsShipmentCounts(
            Open: Sum(x => !x.IsDeleted && x.Status == CustomsShipmentStatus.Open),
            Closed: Sum(x => !x.IsDeleted && x.Status == CustomsShipmentStatus.Closed),
            Inactive: Sum(x => x.IsDeleted)));
    }
}
