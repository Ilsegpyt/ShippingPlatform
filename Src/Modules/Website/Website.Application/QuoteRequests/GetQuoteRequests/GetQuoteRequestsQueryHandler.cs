using BuildingBlocks.Application;
using MediatR;
using Website.Application.Abstractions.Repositories;

namespace Website.Application.QuoteRequests.GetQuoteRequests;

public sealed class GetQuoteRequestsQueryHandler(
    IQuoteRequestRepository quoteRequestRepository)
    : IRequestHandler<
        GetQuoteRequestsQuery,
        Result<PagedResult<Domain.Entities.QuoteRequest>>>
{
    public async Task<Result<PagedResult<Domain.Entities.QuoteRequest>>> Handle(
        GetQuoteRequestsQuery query,
        CancellationToken ct)
    {
        var pageNumber = query.Pagination.PageNumber;
        var pageSize = query.Pagination.PageSize;

        var (items, totalCount) =
            await quoteRequestRepository.GetPagedAsync(
                pageNumber,
                pageSize,
                ct);

        var result =
            new PagedResult<Domain.Entities.QuoteRequest>(
                items,
                totalCount,
                pageNumber,
                pageSize);

        return Result.Success(result);
    }
}