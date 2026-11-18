using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.Ftl.ReopenFtlOperation;

namespace Api.Modules.Operations.Ftl.ReopenFtlOperation;

public static class ReopenFtlOperationEndpoint
{
    public sealed record ReopenRequest(string? Reason);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/operations/ftl/{reference}/reopen", HandleReopen)
            .RequirePermission(PermissionCatalog.FtlOperationsReopen);

        app.MapPut("/api/v1/ftl/operations/{reference}/reopen", HandleReopen)
            .RequirePermission(PermissionCatalog.FtlOperationsReopen);

        app.MapPost("/api/operations/ftl/{reference}/reopen", HandleReopen)
            .RequirePermission(PermissionCatalog.FtlOperationsReopen);
    }

    private static async Task<IResult> HandleReopen(
        string reference,
        [FromBody] ReopenRequest? body,
        ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new ReopenFtlOperationCommand(reference, body?.Reason), ct);
        return result.IsSuccess
            ? Results.Ok(new { success = true })
            : Results.BadRequest(new { error = result.Error });
    }
}
