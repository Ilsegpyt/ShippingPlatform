using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.UpdatePortTerminal;

namespace Api.Modules.Operations.Fcl.UpdatePortTerminal;

public static class UpdatePortTerminalEndpoint
{
    public sealed record UpdatePortTerminalRequest(Guid PortId, string Name, string? Code, string? ArabicName, bool IsActive);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/operations/port-terminals/{id:guid}", async (
            Guid id,
            [FromBody] UpdatePortTerminalRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdatePortTerminalCommand(id, request.PortId, request.Name, request.Code, request.ArabicName, request.IsActive);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(new { success = true })
                : Results.BadRequest(new { error = result.Error });
        })
        .RequirePermission(PermissionCatalog.OperationsEdit);
    }
}
