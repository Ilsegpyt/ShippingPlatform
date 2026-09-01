using Identity.Domain;
using Identity.Infrastructure.Authorization;
using MediatR;
using Schedules.Application.Schedules.DeleteSchedule;

namespace Api.Modules.Schedules;

public static class DeleteScheduleEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/schedules/{id:guid}", async (
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new DeleteScheduleCommand(id);

            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        })
             .RequirePermission(PermissionCatalog.SchedulesDelete);
    }
}