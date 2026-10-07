using BuildingBlocks.Application;
using MediatR;
using Website.Domain.Entities;

namespace Website.Application.QuoteRequests.GetQuoteRequests;

public sealed record GetQuoteRequestsQuery(
    PaginationRequest Pagination)
    : IRequest<Result<PagedResult<QuoteRequest>>>;