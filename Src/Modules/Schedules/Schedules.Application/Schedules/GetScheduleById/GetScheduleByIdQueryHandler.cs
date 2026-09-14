using BuildingBlocks.Application;
using MediatR;
using Schedules.Application.Abstractions;
using Schedules.Application.Queries.GetAllSchedules;

namespace Schedules.Application.Schedules.GetScheduleById;

public sealed class GetScheduleByIdQueryHandler(
    IScheduleRepository repository)
    : IRequestHandler<GetScheduleByIdQuery, Result<ScheduleResponse>>
{
    public async Task<Result<ScheduleResponse>> Handle(
        GetScheduleByIdQuery query,
        CancellationToken ct)
    {
        var schedule = await repository.GetByIdAsync(
            query.Id,
            ct);

        if (schedule is null)
            return Result.Failure<ScheduleResponse>(
                "Schedule not found.");

        var response = new ScheduleResponse(
            schedule.Id,
            schedule.RouteId,
            schedule.Mode,
            schedule.DepartureDate,
            schedule.Vessel,
            schedule.Origin,
            schedule.DeparturePortCode,
            schedule.DepartureCountry,
            schedule.Destination,
            schedule.ArrivalPortCode,
            schedule.ArrivalCountry,
            schedule.Carrier,
            schedule.CarrierCode,
            schedule.VoyageNumber,
            schedule.Arrival,
            schedule.TransitTime,
            schedule.CutoffDate,
            schedule.PortCutoffDate,
            schedule.RateCurrency,
            schedule.ContainerSize,
            schedule.RateAmount,
            schedule.RateRemarks,
            schedule.ValidityDate,
            schedule.FreeTimeAtPOD,
            schedule.FreeTimeAtPOL,
            schedule.TransshipmentData,
            schedule.Notes,
            schedule.CreatedAtUtc,
            schedule.UpdatedAtUtc);

        return Result.Success(response);
    }
}