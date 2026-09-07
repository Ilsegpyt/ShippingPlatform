using BuildingBlocks.Application;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Schedules.Application.Queries.GetAllSchedules;

namespace Api.Modules.Schedules;

public static class GetAllSchedulesEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/schedules", async (
            [AsParameters] PaginationRequest pagination,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new GetAllSchedulesQuery(pagination),
                ct);

            return Results.Ok(result);
        })
        .RequirePermission(PermissionCatalog.SchedulesView);
    }
}