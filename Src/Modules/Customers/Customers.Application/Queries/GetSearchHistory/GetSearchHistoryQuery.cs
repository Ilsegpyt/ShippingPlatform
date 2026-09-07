using BuildingBlocks.Application;
using MediatR;

namespace Customers.Application.Queries.GetSearchHistory;

public sealed record GetSearchHistoryQuery(
    PaginationRequest Pagination)
    : IRequest<Result<PagedResult<SearchHistoryResponse>>>;

public sealed record SearchHistoryResponse(
    Guid Id,
    string Origin,
    string Destination,
    string ContainerSize,
    DateOnly DepartureDate,
    int RoutesFound,
    DateTime SearchedOnUtc);