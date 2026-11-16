using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.CreatePort;

namespace Api.Modules.Operations.CreatePort;

public static class CreatePortEndpoint
{
    public sealed record CreatePortRequest(string Name, string? Code);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/operations/ports", async (
            [FromBody] CreatePortRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreatePortCommand(request.Name, request.Code);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(new { id = result.Value })
                : Results.BadRequest(new { error = result.Error });
        })
        .RequirePermission(PermissionCatalog.OperationsCreate);
    }
}
