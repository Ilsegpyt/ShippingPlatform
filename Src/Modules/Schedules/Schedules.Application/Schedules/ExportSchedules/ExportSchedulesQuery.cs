using BuildingBlocks.Application;
using MediatR;
using Schedules.Domain.Entities;

namespace Schedules.Application.Schedules.ExportSchedules;

public sealed record ExportSchedulesQuery
    : IRequest<Result<IReadOnlyList<Schedule>>>;