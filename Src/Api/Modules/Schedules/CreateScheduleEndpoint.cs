
using Identity.Domain;
using Identity.Infrastructure.Authorization;
using MediatR;
using Schedules.Application.Schedules.CreateSchedule;

namespace Api.Modules.Schedules;

public static class CreateScheduleEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/schedules", async (
            CreateScheduleCommand command,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.SchedulesCreate);
    }
}

