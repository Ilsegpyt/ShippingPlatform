using BuildingBlocks.Application;
using Customers.Application.Abstractions;
using Customers.Domain.SearchHistory;
using MediatR;
using Schedules.Contracts;

namespace Customers.Application.Schedules.SearchCustomerSchedules;

public sealed class SearchCustomerSchedulesQueryHandler(
    IScheduleSearchService scheduleSearchService,
    ISearchHistoryRepository searchHistoryRepository,
    ICustomersUnitOfWork iCustomersUnitOfWork)
    : IRequestHandler<SearchCustomerSchedulesQuery, Result<IReadOnlyList<ScheduleSearchResult>>>
{
    public async Task<Result<IReadOnlyList<ScheduleSearchResult>>> Handle(
        SearchCustomerSchedulesQuery query,
        CancellationToken ct)
    {
        var results = await scheduleSearchService.SearchAsync(
            query.Origin,
            query.Destination,
            query.DepartureDate,
            query.ContainerSize,
            ct);

        var history = SearchHistory.Create(
            query.CustomerId,
            query.Origin,
            query.Destination,
            query.ContainerSize,
            query.DepartureDate,
            results.Count);

        searchHistoryRepository.Add(history);

        await iCustomersUnitOfWork.SaveChangesAsync(ct);

        return Result.Success<IReadOnlyList<ScheduleSearchResult>>(results);
    }
}