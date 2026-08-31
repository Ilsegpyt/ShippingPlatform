using Identity.Domain;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reports.Application.Reports.UploadReport;
using Reports.Domain.Report;
using System.Security.Claims;

namespace Api.Modules.Reports;

public static class UploadReportEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/reports", async (
            [FromForm] Guid customerId,
            [FromForm] string? shipmentRef,
            [FromForm] ReportCategory category,
            [FromForm] ReportService service,
            [FromForm] ReportShipmentType shipmentType,
            IFormFile file,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken ct) =>
        {
            var uploadedByUserId = user.GetUserId();

            await using var stream = file.OpenReadStream();

            var command = new UploadReportCommand(
                customerId,
                shipmentRef,
                category,
                service,
                shipmentType,
                file.FileName,
                stream,
                uploadedByUserId);

            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        })
         .DisableAntiforgery()
        .RequirePermission(PermissionCatalog.ReportsUpload);
    }
}