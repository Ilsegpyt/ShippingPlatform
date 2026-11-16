using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.Ftl.UpdateFtlStatus;

namespace Api.Modules.Operations.UpdateFtlStatus;

public static class UpdateFtlStatusEndpoint
{
    public sealed record UpdateStatusRequest(string? Status, int? Version, string? Ref);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/operations/ftl/{reference}/status", HandleUpdateStatus)
            .RequirePermission(PermissionCatalog.OperationsEdit);

        // Support front routes
        app.MapPut("/api/v1/ftl/operations/{reference}/status", HandleUpdateStatus)
            .RequirePermission(PermissionCatalog.OperationsEdit);
        app.MapPost("/updateShipmentStatus", HandleUpdateStatusFromLegacy)
            .RequirePermission(PermissionCatalog.OperationsEdit);
    }

    private static async Task<IResult> HandleUpdateStatus(
        string reference,
        [FromBody] UpdateStatusRequest body,
        ISender sender,
        CancellationToken ct)
    {
        var command = new UpdateFtlStatusCommand(reference, body.Status ?? "Open", body.Version);
        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }

    private static async Task<IResult> HandleUpdateStatusFromLegacy(
        [FromBody] UpdateStatusRequest body,
        ISender sender,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(body.Ref))
            return Results.BadRequest(new { error = "Reference is required." });

        var command = new UpdateFtlStatusCommand(body.Ref, body.Status ?? "Open", body.Version);
        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}

