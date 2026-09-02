using Schedules.Application.Abstractions;
using Schedules.Contracts;
using Schedules.Domain.Schedule;

namespace Schedules.Infrastructure.Services;

public sealed class ScheduleSearchService(
    IScheduleRepository scheduleRepository)
    : IScheduleSearchService
{
    public async Task<IReadOnlyList<ScheduleSearchResult>> SearchAsync(
        string origin,
        string destination,
        DateOnly departureDate,
        string containerSize,
        CancellationToken ct)
    {
        if (!Enum.TryParse<ContainerSize>(
                containerSize,
                ignoreCase: true,
                out var parsedContainerSize))
        {
            return [];
        }

        var schedules = await scheduleRepository.SearchAsync(
            origin,
            destination,
            departureDate,
            parsedContainerSize,
            ct);

        return schedules
            .Select(schedule => new ScheduleSearchResult(
                schedule.Id,
                schedule.RouteId,
                schedule.Mode.ToString(),
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
                schedule.ContainerSize.ToString()))
            .ToList();
    }
}