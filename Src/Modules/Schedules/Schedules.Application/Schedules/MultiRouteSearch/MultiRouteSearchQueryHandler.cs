using BuildingBlocks.Application;
using MediatR;
using Schedules.Contracts;

namespace Schedules.Application.Schedules.MultiRouteSearch;

public sealed class MultiRouteSearchQueryHandler(
    IScheduleSearchService scheduleSearchService)
    : IRequestHandler<
        MultiRouteSearchQuery,
        Result<IReadOnlyList<ScheduleSearchResult>>>
{
    public async Task<Result<IReadOnlyList<ScheduleSearchResult>>> Handle(
        MultiRouteSearchQuery query,
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
        }

        return Result.Success<IReadOnlyList<ScheduleSearchResult>>(
            allResults);
    }
}