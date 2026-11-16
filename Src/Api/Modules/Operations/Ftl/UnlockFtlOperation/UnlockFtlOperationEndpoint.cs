using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.Ftl.UnlockFtlOperation;

namespace Api.Modules.Operations.UnlockFtlOperation;

public static class UnlockFtlOperationEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/operations/ftl/{reference}/unlock", HandleUnlock)
            .RequirePermission(PermissionCatalog.OperationsEdit);

        // Support front route
        app.MapPost("/api/v1/ftl/operations/{reference}/unlock", HandleUnlock)
            .RequirePermission(PermissionCatalog.OperationsEdit);
    }

    private static async Task<IResult> HandleUnlock(
        string reference,
        ISender sender,
        CancellationToken ct)
    {
        var command = new UnlockFtlOperationCommand(reference);
        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { error = result.Error });
    }
}

