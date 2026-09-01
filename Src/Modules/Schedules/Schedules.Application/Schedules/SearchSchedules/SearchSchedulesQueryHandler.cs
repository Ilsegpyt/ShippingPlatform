using BuildingBlocks.Application;
using MediatR;
using Schedules.Application.Abstractions;

namespace Schedules.Application.Schedules.SearchSchedules;

public sealed class SearchSchedulesQueryHandler(
    IScheduleRepository scheduleRepository)
    : IRequestHandler<
        SearchSchedulesQuery,
        Result<IReadOnlyList<SearchScheduleResponse>>>
{
    public async Task<Result<IReadOnlyList<SearchScheduleResponse>>> Handle(
        SearchSchedulesQuery query,
        CancellationToken ct)
    {
        var schedules = await scheduleRepository.SearchAsync(
            query.Origin,
            query.Destination,
            query.DepartureDate,
            query.ContainerSize,
            ct);

        var response = schedules
            .Select(schedule => new SearchScheduleResponse(
                schedule.Id,
                schedule.RouteId,
                schedule.Mode,
                schedule.DepartureDate,
                schedule.Vessel,
                schedule.Origin,
                schedule.DeparturePortCode,
                schedule.Destination,
                schedule.ArrivalPortCode,
                schedule.Carrier,
                schedule.CarrierCode,
                schedule.VoyageNumber,
                schedule.Arrival,
                schedule.TransitTime,
                schedule.RateAmount,
                schedule.RateCurrency,
                schedule.ContainerSize))
            .ToList();

        return Result.Success<IReadOnlyList<SearchScheduleResponse>>(
            response);
    }
}
