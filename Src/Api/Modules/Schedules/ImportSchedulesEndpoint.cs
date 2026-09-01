using Identity.Domain;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Schedules.Application.Schedules.ImportSchedules;

namespace Api.Modules.Schedules;

public static class ImportSchedulesEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/schedules/import", async (
            [FromForm] IFormFile file,
            ISender sender,
            CancellationToken ct) =>
        {
            await using var stream = file.OpenReadStream();

            var command = new ImportSchedulesCommand(stream);

            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        }).DisableAntiforgery().RequirePermission(PermissionCatalog.SchedulesImport);
    }
}