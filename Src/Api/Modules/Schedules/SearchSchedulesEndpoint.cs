using Identity.Domain;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Schedules.Application.Schedules.SearchSchedules;
using Schedules.Domain.Schedule;

namespace Api.Modules.Schedules;

public static class SearchSchedulesEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/schedules/search", async (
            [FromQuery] string origin,
            [FromQuery] string destination,
            [FromQuery] DateOnly departureDate,
            [FromQuery] ContainerSize containerSize,
            ISender sender,
            CancellationToken ct) =>
        {
            var query = new SearchSchedulesQuery(
                origin,
                destination,
                departureDate,
                containerSize);

            var result = await sender.Send(query, ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        }).RequirePermission(PermissionCatalog.SchedulesSearch);
    }
}