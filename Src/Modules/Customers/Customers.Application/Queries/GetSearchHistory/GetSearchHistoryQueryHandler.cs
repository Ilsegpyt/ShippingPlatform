using BuildingBlocks.Application;
using Customers.Application.Abstractions;
using MediatR;

namespace Customers.Application.Queries.GetSearchHistory;

public sealed class GetSearchHistoryQueryHandler(
    ISearchHistoryRepository repository)
    : IRequestHandler<GetSearchHistoryQuery, Result<PagedResult<SearchHistoryResponse>>>
{
    public async Task<Result<PagedResult<SearchHistoryResponse>>> Handle(
        GetSearchHistoryQuery query,
        CancellationToken ct)
    {
        var page = query.Pagination.PageNumber;
        var pageSize = query.Pagination.PageSize;
        var skip = (page - 1) * pageSize;

        var totalCount = await repository.CountAsync(ct);

        var history = await repository.GetAllAsync(
            skip,
            pageSize,
            ct);

        var items = history
            .Select(x => new SearchHistoryResponse(
                x.Id,
                x.Origin,
                x.Destination,
                x.ContainerSize,
                x.DepartureDate,
                x.RoutesFound,
                x.SearchedOnUtc))
            .ToList();

        return Result.Success(
            new PagedResult<SearchHistoryResponse>(
                items,
                totalCount,
                page,
                pageSize));
    }
}