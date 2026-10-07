using BuildingBlocks.Application;
using MediatR;
using Website.Application.Abstractions.Repositories;
using Website.Domain.Entities;

namespace Website.Application.QuoteRequests.GetQuoteRequestById;

public sealed class GetQuoteRequestByIdQueryHandler(
    IQuoteRequestRepository quoteRequestRepository)
    : IRequestHandler<
        GetQuoteRequestByIdQuery,
        Result<QuoteRequest>>
{
    public async Task<Result<QuoteRequest>> Handle(
        GetQuoteRequestByIdQuery query,
        CancellationToken ct)
    {
        var request =
            await quoteRequestRepository.GetByIdAsync(
                query.Id,
                ct);

        if (request is null)
            return Result.Failure<QuoteRequest>(
                "Quote request was not found.");

        return Result.Success(request);
    }
}