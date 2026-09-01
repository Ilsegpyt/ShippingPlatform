using BuildingBlocks.Application;
using MediatR;
using Schedules.Application.Abstractions;

namespace Schedules.Application.Schedules.CreateSchedule;

public sealed class CreateScheduleCommandHandler(
    IScheduleRepository scheduleRepository,
    ISchedulesUnitOfWork unitOfWork)
    : IRequestHandler<CreateScheduleCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateScheduleCommand command,
        CancellationToken ct)
    {
        var schedule = Domain.Schedule.Schedule.Create(
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

        await scheduleRepository.AddAsync(schedule, ct);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(schedule.Id);
    }
}
