using BuildingBlocks.Application;
using MediatR;
using Schedules.Application.Queries.GetAllSchedules;

namespace Schedules.Application.Schedules.GetScheduleById;

public sealed record GetScheduleByIdQuery(Guid Id)
    : IRequest<Result<ScheduleResponse>>;