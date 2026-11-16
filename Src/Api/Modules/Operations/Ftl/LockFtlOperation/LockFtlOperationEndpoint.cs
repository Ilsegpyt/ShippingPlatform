using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.Ftl.LockFtlOperation;

namespace Api.Modules.Operations.LockFtlOperation;

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

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}

