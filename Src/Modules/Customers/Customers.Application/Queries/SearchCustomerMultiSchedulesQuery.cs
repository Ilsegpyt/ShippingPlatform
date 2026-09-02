using BuildingBlocks.Application;
using Customers.Application.Abstractions;
using Customers.Domain.SearchHistory;
using MediatR;
using Schedules.Contracts;

namespace Customers.Application.Queries;

public sealed record SearchCustomerMultiSchedulesQuery(
    IReadOnlyList<SearchCustomerMultiRouteItem> Routes,
    Guid CustomerId)
    : IRequest<Result<IReadOnlyList<ScheduleSearchResult>>>;

public sealed record SearchCustomerMultiRouteItem(
    string Origin,
    string Destination,
    DateOnly DepartureDate,
    string ContainerSize);

public sealed class SearchCustomerMultiSchedulesQueryHandler(
    IScheduleSearchService scheduleSearchService,
    ISearchHistoryRepository searchHistoryRepository,
    ICustomersUnitOfWork unitOfWork)
    : IRequestHandler<
        SearchCustomerMultiSchedulesQuery,
        Result<IReadOnlyList<ScheduleSearchResult>>>
{
    public async Task<Result<IReadOnlyList<ScheduleSearchResult>>> Handle(
        SearchCustomerMultiSchedulesQuery query,
        CancellationToken ct)
    {
        var allResults = new List<ScheduleSearchResult>();

        foreach (var route in query.Routes)
        {
            var results = await scheduleSearchService.SearchAsync(
                route.Origin,
                route.Destination,
                route.DepartureDate,
                route.ContainerSize,
                ct);

            allResults.AddRange(results);

            var history = SearchHistory.Create(
                query.CustomerId,
                route.Origin,
                route.Destination,
                route.ContainerSize,
                route.DepartureDate,
                results.Count);

            await searchHistoryRepository.AddAsync(
                history,
                ct);
        }

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success<IReadOnlyList<ScheduleSearchResult>>(
            allResults);
    }
}