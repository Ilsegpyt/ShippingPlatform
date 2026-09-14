using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Schedules.Application.Schedules.GetScheduleById;

namespace Api.Modules.Schedules;

public static class GetScheduleByIdEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/schedules/{id:guid}", async (
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new GetScheduleByIdQuery(id),
                ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound(result.Error);
        })
        .RequirePermission(PermissionCatalog.SchedulesView);
    }
}