using Identity.Domain;
using Identity.Infrastructure.Authorization;
using MediatR;
using Schedules.Application.Abstractions;
using Schedules.Application.Schedules.ExportSchedules;

namespace Api.Modules.Schedules;

public static class ExportSchedulesEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/schedules/export", async (
            ISender sender,
            IScheduleExcelWriter excelWriter,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new ExportSchedulesQuery(),
                ct);

            if (!result.IsSuccess)
                return Results.BadRequest(result.Error);

            var fileBytes = excelWriter.Write(result.Value);

            return Results.File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "schedules.xlsx");
        }).RequirePermission(PermissionCatalog.SchedulesExport);
    }
}