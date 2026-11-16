using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.Ftl.UpdateFtlOperation;

namespace Api.Modules.Operations.UpdateFtlOperation;

public static class UpdateFtlOperationEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/operations/ftl", HandleUpdate)
            .RequirePermission(PermissionCatalog.OperationsEdit);
        app.MapPost("/api/operations/ftl/save", HandleUpdate)
            .RequirePermission(PermissionCatalog.OperationsEdit);

        // Support front routes
        app.MapPost("/api/v1/ftl/workspace", HandleUpdate)
            .RequirePermission(PermissionCatalog.OperationsEdit);
        app.MapPost("/saveWorkspace", HandleUpdate)
            .RequirePermission(PermissionCatalog.OperationsEdit);
    }

    private static async Task<IResult> HandleUpdate(
        [FromBody] UpdateFtlOperationCommand command,
        ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        if (result.IsSuccess)
            return Results.Ok(result.Value);

        if (result.Error.Contains("conflict", StringComparison.OrdinalIgnoreCase) ||
            result.Error.Contains("changed on the server", StringComparison.OrdinalIgnoreCase))
            return Results.Conflict(new { error = result.Error });

        return Results.BadRequest(new { error = result.Error });
    }
}
