using BuildingBlocks.Application;
using MediatR;
using Schedules.Application.Abstractions;
using Schedules.Domain.Schedule;

namespace Schedules.Application.Schedules.ExportSchedules;

public sealed class ExportSchedulesQueryHandler(
    IScheduleRepository scheduleRepository)
    : IRequestHandler<
        ExportSchedulesQuery,
        Result<IReadOnlyList<Schedule>>>
{
    public async Task<Result<IReadOnlyList<Schedule>>> Handle(
        ExportSchedulesQuery query,
        CancellationToken ct)
    {
        var schedules = await scheduleRepository.GetAllAsync(ct);

        return Result.Success(schedules);
    }
}