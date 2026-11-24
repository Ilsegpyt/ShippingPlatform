using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.Ftl.CreateFtlTerminal;

namespace Api.Modules.Operations.Ftl.CreateFtlTerminal;

public static class CreateFtlTerminalEndpoint
{
    public sealed record CreateFtlTerminalRequest(string Name);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/operations/ftl/terminals", async (
            [FromBody] CreateFtlTerminalRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreateFtlTerminalCommand(request.Name);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(new { id = result.Value })
                : Results.BadRequest(new { error = result.Error });
        })
        .RequirePermission(PermissionCatalog.FtlOperationsCreate);
    }
}
