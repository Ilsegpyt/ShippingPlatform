using BuildingBlocks.Application;
using MediatR;
using Schedules.Application.Abstractions;

namespace Schedules.Application.Schedules.DeleteSchedule;

public sealed class DeleteScheduleCommandHandler(
    IScheduleRepository scheduleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteScheduleCommand, Result>
{
    public async Task<Result> Handle(
        DeleteScheduleCommand command,
        CancellationToken ct)
    {
        var schedule = await scheduleRepository.GetByIdAsync(
            command.Id,
            ct);

        if (schedule is null)
            return Result.Failure("Schedule not found.");

        scheduleRepository.Remove(schedule);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}