using System.Security.Claims;
using Identity.Infrastructure.Authorization;
using MediatR;
using Reports.Application.Reports.DownloadReport;
using Reports.Application.Abstractions;
using Identity.Domain;

namespace Api.Modules.Reports;

public static class DownloadReportEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/reports/{id:guid}/file", async (
            Guid id,
            ClaimsPrincipal user,
            ISender sender,
            IReportFileStorage fileStorage,
            CancellationToken ct) =>
        {
            var userId = user.GetUserId();

            var result = await sender.Send(
                new DownloadReportQuery(id, userId),
                ct);

            if (result is null)
                return Results.NotFound();

            var stream = await fileStorage.OpenReadAsync(
                result.StorageKey,
                ct);

            return Results.File(
                stream,
                GetContentType(result.FileName),
                result.FileName);
        })
        .RequirePermission(PermissionCatalog.ReportsView);
    }

    private static string GetContentType(string fileName)
    {
        return Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".pdf" => "application/pdf",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".xls" => "application/vnd.ms-excel",
            ".csv" => "text/csv",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => "application/octet-stream"
        };
    }
}