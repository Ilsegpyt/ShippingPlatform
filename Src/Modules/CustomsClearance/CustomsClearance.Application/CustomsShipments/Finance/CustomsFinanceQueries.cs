using BuildingBlocks.Application;
using CustomsClearance.Application.Abstractions.Repositories;
using MediatR;

namespace CustomsClearance.Application.CustomsShipments.Finance;

// One tab of the Customs Finance screen, paged (page size 1..100).
public sealed record GetCustomsFinanceQuery(CustomsFinanceTab Tab, PaginationRequest Pagination)
    : IRequest<Result<CustomsFinanceList>>;

public sealed class GetCustomsFinanceQueryHandler(
    ICustomsFinanceRepository customsFinanceRepository,
    TimeProvider clock)
    : IRequestHandler<GetCustomsFinanceQuery, Result<CustomsFinanceList>>
{
    public async Task<Result<CustomsFinanceList>> Handle(
        GetCustomsFinanceQuery query,
        CancellationToken ct)
    {
        var today = CustomsCairoTime.Today(clock);
        var page = Math.Max(1, query.Pagination.PageNumber);
        var size = Math.Clamp(query.Pagination.PageSize, 1, 100);

        var (items, totalCount) = await customsFinanceRepository.GetPagedAsync(
            query.Tab,
            today,
            query.Pagination.Search,
            page,
            size,
            ct);

        // No amounts are stored yet, so every total is null.
        var totals = new CustomsFinanceTotals(null, null, null);

        return Result.Success(
            new CustomsFinanceList(items, totalCount, page, size, today, totals));
    }
}

// Badge counts for the 4 lists.
public sealed record GetCustomsFinanceCountsQuery : IRequest<Result<CustomsFinanceCounts>>;

public sealed class GetCustomsFinanceCountsQueryHandler(
    ICustomsFinanceRepository customsFinanceRepository,
    TimeProvider clock)
    : IRequestHandler<GetCustomsFinanceCountsQuery, Result<CustomsFinanceCounts>>
{
    public async Task<Result<CustomsFinanceCounts>> Handle(
        GetCustomsFinanceCountsQuery query,
        CancellationToken ct)
    {
        var today = CustomsCairoTime.Today(clock);

        // Sequential: one DbContext cannot run queries in parallel.
        var forecast = await customsFinanceRepository.CountAsync(CustomsFinanceTab.Forecast, today, ct);
        var request = await customsFinanceRepository.CountAsync(CustomsFinanceTab.Request, today, ct);
        var approved = await customsFinanceRepository.CountAsync(CustomsFinanceTab.Approved, today, ct);
        var excluded = await customsFinanceRepository.CountAsync(CustomsFinanceTab.Excluded, today, ct);

        return Result.Success(
            new CustomsFinanceCounts(today, forecast, request, approved, excluded));
    }
}
