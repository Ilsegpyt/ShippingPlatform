using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.CreatePortTerminal;

namespace Api.Modules.Operations.Fcl.CreatePortTerminal;

public static class CreatePortTerminalEndpoint
{
    public sealed record CreatePortTerminalRequest(Guid PortId, string Name, string? Code, string? ArabicName);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/operations/port-terminals", async (
            [FromBody] CreatePortTerminalRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreatePortTerminalCommand(request.PortId, request.Name, request.Code, request.ArabicName);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(new { id = result.Value })
                : Results.BadRequest(new { error = result.Error });
        })
        .RequirePermission(PermissionCatalog.OperationsCreate);
    }
}
