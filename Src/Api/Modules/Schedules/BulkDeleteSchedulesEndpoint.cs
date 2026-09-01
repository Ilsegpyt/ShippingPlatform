using Identity.Domain;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Schedules.Application.Schedules.BulkDeleteSchedules;

namespace Api.Modules.Schedules;

public static class BulkDeleteSchedulesEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/schedules", async (
           [FromBody] BulkDeleteSchedulesRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new BulkDeleteSchedulesCommand(request.Ids);

            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        }).RequirePermission(PermissionCatalog.SchedulesDelete);
    }
}

public sealed record BulkDeleteSchedulesRequest(
    IReadOnlyList<Guid> Ids);