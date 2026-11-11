using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.Ftl.DeleteFtlOperation;

namespace Api.Modules.Operations.DeleteFtlOperation;

public static class DeleteFtlOperationEndpoint
{
    public sealed record DeleteRequest(int? Version, string? Ref);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/operations/ftl/{reference}", HandleDelete)
            .RequirePermission(PermissionCatalog.OperationsDelete);

        // Support front routes
        app.MapDelete("/api/v1/ftl/operations/{reference}", HandleDelete)
            .RequirePermission(PermissionCatalog.OperationsDelete);
        app.MapPost("/deleteShipment", HandleDeleteFromLegacy)
            .RequirePermission(PermissionCatalog.OperationsDelete);
    }

    private static async Task<IResult> HandleDelete(
        string reference,
        [FromBody] DeleteRequest? body,
        ISender sender,
        CancellationToken ct)
    {
        var command = new DeleteFtlOperationCommand(reference, body?.Version);
        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }

    private static async Task<IResult> HandleDeleteFromLegacy(
        [FromBody] DeleteRequest body,
        ISender sender,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(body.Ref))
            return Results.BadRequest(new { error = "Reference is required." });

        var command = new DeleteFtlOperationCommand(body.Ref, body.Version);
        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}

