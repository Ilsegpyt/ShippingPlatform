using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.Ftl.Common;
using Operations.Application.Operations.Ftl.UpdateFtlOperation;

namespace Api.Modules.Operations.Ftl.UpdateFtlOperation;

public static class UpdateFtlOperationEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        // Per-operation single update (Identical to FCL)
        app.MapPut("/api/v1/ftl/operations/{reference}", HandleUpdateSingle)
            .RequirePermission(PermissionCatalog.OperationsEdit);
        app.MapPut("/api/operations/ftl/{reference}", HandleUpdateSingle)
            .RequirePermission(PermissionCatalog.OperationsEdit);
    }

    private static async Task<IResult> HandleUpdateSingle(
        string reference,
        [FromBody] FtlOperationDto operation,
        ISender sender,
        CancellationToken ct)
    {
        var op = operation with { Ref = reference };
        var command = new UpdateFtlOperationCommand(null, new List<FtlOperationDto> { op });
        return await ExecuteUpdate(command, sender, ct);
    }

    private static async Task<IResult> HandleUpdate(
        [FromBody] UpdateFtlOperationCommand command,
        ISender sender,
        CancellationToken ct)
    {
        return await ExecuteUpdate(command, sender, ct);
    }

    private static async Task<IResult> ExecuteUpdate(
        UpdateFtlOperationCommand command,
        ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        if (result.IsSuccess)
            return Results.Ok(result.Value);

        if (result.Error.Contains("conflict", StringComparison.OrdinalIgnoreCase) ||
            result.Error.Contains("changed on the server", StringComparison.OrdinalIgnoreCase) ||
            result.Error.Contains("locked", StringComparison.OrdinalIgnoreCase))
            return Results.Conflict(new { error = result.Error });

        return Results.BadRequest(new { error = result.Error });
    }
}
