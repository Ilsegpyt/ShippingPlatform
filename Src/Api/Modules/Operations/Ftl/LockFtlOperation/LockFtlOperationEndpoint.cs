using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.Ftl.LockFtlOperation;

namespace Api.Modules.Operations.Ftl.LockFtlOperation;

public static class LockFtlOperationEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/operations/ftl/{reference}/lock", HandleLock)
            .RequirePermission(PermissionCatalog.OperationsEdit);

        // Support front route
        app.MapPost("/api/v1/ftl/operations/{reference}/lock", HandleLock)
            .RequirePermission(PermissionCatalog.OperationsEdit);
    }

    private static async Task<IResult> HandleLock(
        string reference,
        ISender sender,
        CancellationToken ct)
    {
        var command = new LockFtlOperationCommand(reference);
        var result = await sender.Send(command, ct);

        if (result.IsSuccess)
            return Results.Ok(result.Value);

        if (result.Error.Contains("locked", StringComparison.OrdinalIgnoreCase))
            return Results.Conflict(new { error = result.Error });

        return Results.BadRequest(new { error = result.Error });
    }
}

