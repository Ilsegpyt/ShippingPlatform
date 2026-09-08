using BuildingBlocks.Application;
using MediatR;


namespace Schedules.Application.Schedules.DeleteSchedule;

public sealed record DeleteSchedulesCommand(
    IReadOnlyCollection<Guid> ScheduleIds
) : IRequest<Result>;