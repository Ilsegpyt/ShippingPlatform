using BuildingBlocks.Application;
using Customers.Application.Abstractions;
using Customers.Domain.SearchHistory;
using MediatR;
using Schedules.Contracts;

namespace Customers.Application.Queries;

public sealed record SearchCustomerSchedulesQuery(
    string Origin,
    string Destination,
    DateOnly DepartureDate,
    string ContainerSize,
    Guid CustomerId
) : IRequest<Result<IReadOnlyList<ScheduleSearchResult>>>;

public sealed class SearchCustomerSchedulesQueryHandler(
    IScheduleSearchService scheduleSearchService,
    ISearchHistoryRepository searchHistoryRepository,
    ICustomersUnitOfWork unitOfWork)
    : IRequestHandler<
        SearchCustomerSchedulesQuery,
        Result<IReadOnlyList<ScheduleSearchResult>>>
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

        await searchHistoryRepository.AddAsync(history, ct);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success<IReadOnlyList<ScheduleSearchResult>>(results);
    }
}