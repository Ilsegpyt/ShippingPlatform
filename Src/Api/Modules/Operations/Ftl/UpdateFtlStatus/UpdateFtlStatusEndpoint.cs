using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Abstractions;
using Operations.Application.Operations.Ftl.UpdateFtlStatus;

namespace Api.Modules.Operations.Ftl.UpdateFtlStatus;

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

        // Status Audit History
        app.MapGet("/api/v1/ftl/operations/{reference}/status-audits", HandleGetStatusAudits)
            .RequirePermission(PermissionCatalog.OperationsView);
        app.MapGet("/api/operations/ftl/{reference}/status-audits", HandleGetStatusAudits)
            .RequirePermission(PermissionCatalog.OperationsView);
    }

    private static async Task<IResult> HandleGetStatusAudits(
        string reference,
        IFtlOperationRepository repository,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(reference))
            return Results.BadRequest(new { error = "Reference is required." });

        var operation = await repository.GetByRefAsync(reference, true, ct);
        if (operation is null && Guid.TryParse(reference, out var guidId))
        {
            operation = await repository.GetByIdAsync(guidId, true, ct);
        }

        if (operation is null)
            return Results.NotFound(new { error = $"Operation '{reference}' not found." });

        var audits = operation.StatusAudits
            .OrderByDescending(a => a.AtUtc)
            .Select(a => new
            {
                id = a.Id,
                ftlOperationId = a.FtlOperationId,
                beforeStatus = a.BeforeStatus.ToString(),
                afterStatus = a.AfterStatus.ToString(),
                userId = a.UserId,
                userName = a.UserId == operation.CreatedByUserId
                    ? operation.CreatedByName
                    : (a.UserId == operation.EnteredByUserId ? operation.EnteredByName : a.UserId),
                atUtc = a.AtUtc
            })
            .ToList();

        return Results.Ok(audits);
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

