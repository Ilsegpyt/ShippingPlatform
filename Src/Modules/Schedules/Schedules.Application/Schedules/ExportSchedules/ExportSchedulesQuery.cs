using BuildingBlocks.Application;
using MediatR;
using Schedules.Domain.Schedule;

namespace Schedules.Application.Schedules.ExportSchedules;

public sealed record ExportSchedulesQuery
    : IRequest<Result<IReadOnlyList<Schedule>>>;