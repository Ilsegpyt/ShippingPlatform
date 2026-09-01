using BuildingBlocks.Application;
using MediatR;
using Schedules.Application.Abstractions;

namespace Schedules.Application.Schedules.BulkDeleteSchedules;

public sealed class BulkDeleteSchedulesCommandHandler(
    IScheduleRepository scheduleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<BulkDeleteSchedulesCommand, Result>
{
    public async Task<Result> Handle(
        BulkDeleteSchedulesCommand command,
        CancellationToken ct)
    {
        foreach (var id in command.Ids)
        {
            var schedule = await scheduleRepository.GetByIdAsync(
                id,
                ct);

            if (schedule is not null)
            {
                scheduleRepository.Remove(schedule);
            }
        }

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}