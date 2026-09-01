using BuildingBlocks.Application;
using MediatR;
using Schedules.Application.Abstractions;
using Schedules.Domain.Schedule;

namespace Schedules.Application.Schedules.ExportSearchResults;

public sealed class ExportSearchResultsQueryHandler(
    IScheduleRepository scheduleRepository)
    : IRequestHandler<
        ExportSearchResultsQuery,
        Result<IReadOnlyList<Schedule>>>
{
    public async Task<Result<IReadOnlyList<Schedule>>> Handle(
        ExportSearchResultsQuery query,
        CancellationToken ct)
    {
        var schedules = await scheduleRepository.SearchAsync(
            query.Origin,
            query.Destination,
            query.DepartureDate,
            query.ContainerSize,
            ct);

        return Result.Success(schedules);
    }
}