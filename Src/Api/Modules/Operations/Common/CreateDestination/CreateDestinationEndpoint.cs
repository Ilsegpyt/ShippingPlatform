using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.CreateDestination;

namespace Api.Modules.Operations.CreateDestination;

public static class CreateDestinationEndpoint
{
    public sealed record CreateDestinationRequest(string Name);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/operations/destinations", async (
            [FromBody] CreateDestinationRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreateDestinationCommand(request.Name);
            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.Ok(new { id = result.Value })
                : Results.BadRequest(new { error = result.Error });
        })
        .RequirePermission(PermissionCatalog.OperationsCreate);
    }
}
