using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Schedules.Application.Schedules.DeleteSchedule;

namespace Api.Modules.Schedules;

public static class DeleteScheduleEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/schedules", async (
            [FromBody] DeleteSchedulesCommand command,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.SchedulesDelete);
    }
}