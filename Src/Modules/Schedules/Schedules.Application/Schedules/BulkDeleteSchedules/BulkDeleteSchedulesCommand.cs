using BuildingBlocks.Application;
using MediatR;

namespace Schedules.Application.Schedules.BulkDeleteSchedules;

public sealed record BulkDeleteSchedulesCommand(
    IReadOnlyList<Guid> Ids
) : IRequest<Result>;