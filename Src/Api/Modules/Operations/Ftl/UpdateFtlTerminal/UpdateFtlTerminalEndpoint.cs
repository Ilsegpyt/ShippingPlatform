using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.Ftl.UpdateFtlTerminal;

namespace Api.Modules.Operations.Ftl.UpdateFtlTerminal;

public static class UpdateFtlTerminalEndpoint
{
    public sealed record UpdateFtlTerminalRequest(string Name, bool IsActive);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/operations/ftl/terminals/{id:guid}", async (
            Guid id,
            [FromBody] UpdateFtlTerminalRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateFtlTerminalCommand(id, request.Name, request.IsActive);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(new { success = true })
                : Results.BadRequest(new { error = result.Error });
        })
        .RequirePermission(PermissionCatalog.FtlOperationsEdit);
    }
}
