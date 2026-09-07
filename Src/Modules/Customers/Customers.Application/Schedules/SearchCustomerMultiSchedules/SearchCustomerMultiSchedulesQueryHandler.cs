using BuildingBlocks.Application;
using Customers.Application.Abstractions;
using Customers.Domain.SearchHistory;
using MediatR;
using Schedules.Contracts;

namespace Customers.Application.Schedules.SearchCustomerMultiSchedules;

public sealed class SearchCustomerMultiSchedulesQueryHandler(
    IScheduleSearchService scheduleSearchService,
    ISearchHistoryRepository searchHistoryRepository,
    ICustomersUnitOfWork iCustomersUnitOfWork)
    : IRequestHandler<SearchCustomerMultiSchedulesQuery, Result<IReadOnlyList<ScheduleSearchResult>>>


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

            searchHistoryRepository.Add(history);


        }

        await iCustomersUnitOfWork.SaveChangesAsync(ct);

        return Result.Success<IReadOnlyList<ScheduleSearchResult>>(
            allResults);
    }
}