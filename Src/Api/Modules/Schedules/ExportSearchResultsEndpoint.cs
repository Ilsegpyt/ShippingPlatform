using Identity.Domain;
using Identity.Infrastructure.Authorization;
using MediatR;
using Schedules.Application.Abstractions;
using Schedules.Application.Schedules.ExportSearchResults;
using Schedules.Domain.Schedule;

namespace Api.Modules.Schedules;

public static class ExportSearchResultsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/schedules/export/search", async (
            string origin,
            string destination,
            DateOnly departureDate,
            ContainerSize containerSize,
            ISender sender,
            IScheduleExcelWriter excelWriter,
            CancellationToken ct) =>
        {
            var query = new ExportSearchResultsQuery(
                origin,
                destination,
                departureDate,
                containerSize);

            var result = await sender.Send(query, ct);

            if (!result.IsSuccess)
                return Results.BadRequest(result.Error);

            var fileBytes = excelWriter.Write(result.Value);

            return Results.File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "schedules-search-results.xlsx");
        }).RequirePermission(PermissionCatalog.SchedulesExport);
    }
}