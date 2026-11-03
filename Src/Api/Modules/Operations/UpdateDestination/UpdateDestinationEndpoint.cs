using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.UpdateDestination;

namespace Api.Modules.Operations.UpdateDestination;

public static class UpdateDestinationEndpoint
{
    public sealed record UpdateDestinationRequest(string Name, bool IsActive);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/operations/destinations/{id:guid}", async (
            Guid id,
            [FromBody] UpdateDestinationRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateDestinationCommand(id, request.Name, request.IsActive);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(new { success = true })
                : Results.BadRequest(new { error = result.Error });
        })
        .RequirePermission(PermissionCatalog.OperationsEdit);
    }
}
