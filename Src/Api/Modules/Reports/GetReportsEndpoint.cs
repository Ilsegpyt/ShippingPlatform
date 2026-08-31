using System.Security.Claims;
using BuildingBlocks.Application.Contracts;
using Identity.Domain;
using Identity.Infrastructure.Authorization;
using MediatR;
using Reports.Application.Reports.GetReports;

namespace Api.Modules.Reports;

public static class GetReportsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/reports", async (
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken ct) =>
        {
            var userId = user.GetUserId();

            var result = await sender.Send(
                new GetReportsQuery(userId),
                ct);

            return Results.Ok(result);
        })
        .RequirePermission(PermissionCatalog.ReportsView);
    }
}