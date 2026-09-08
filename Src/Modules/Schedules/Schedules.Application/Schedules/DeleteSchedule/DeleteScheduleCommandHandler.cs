using BuildingBlocks.Application;
using MediatR;
using Schedules.Application.Abstractions;

namespace Schedules.Application.Schedules.DeleteSchedule;

public sealed class DeleteSchedulesCommandHandler(
    IScheduleRepository scheduleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteSchedulesCommand, Result>
{
    public async Task<Result> Handle(
        DeleteSchedulesCommand command,
        CancellationToken ct)
    {
        foreach (var scheduleId in command.ScheduleIds)
        {
            var schedule = await scheduleRepository.GetByIdAsync(
                scheduleId,
                ct);

            if (schedule is null)
                return Result.Failure($"Schedule '{scheduleId}' not found.");

            scheduleRepository.Remove(schedule);
        }

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}