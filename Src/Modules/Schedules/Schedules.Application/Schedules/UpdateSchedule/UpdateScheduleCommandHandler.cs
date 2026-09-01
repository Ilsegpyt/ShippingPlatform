using BuildingBlocks.Application;
using MediatR;
using Schedules.Application.Abstractions;

namespace Schedules.Application.Schedules.UpdateSchedule;

public sealed class UpdateScheduleCommandHandler(
    IScheduleRepository scheduleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateScheduleCommand, Result>
{
    public async Task<Result> Handle(
        UpdateScheduleCommand command,
        CancellationToken ct)
    {
        var schedule = await scheduleRepository.GetByIdAsync(
            command.Id,
            ct);

        if (schedule is null)
        {
            return Result.Failure(
                "Schedule not found.");
        }

        schedule.Patch(
            command.RouteId,
            command.Mode,
            command.DepartureDate,
            command.Vessel,
            command.Origin,
            command.DeparturePortCode,
            command.DepartureCountry,
            command.Destination,
            command.ArrivalPortCode,
            command.ArrivalCountry,
            command.Carrier,
            command.CarrierCode,
            command.VoyageNumber,
            command.Arrival,
            command.TransitTime,
            command.CutoffDate,
            command.RateCurrency,
            command.ContainerSize,
            command.RateAmount,
            command.RateRemarks,
            command.ValidityDate,
            command.FreeTimeAtPOD,
            command.FreeTimeAtPOL,
            command.TransshipmentData,
            command.Notes);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}