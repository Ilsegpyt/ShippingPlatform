using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.UpdatePort;

namespace Api.Modules.Operations.UpdatePort;

public static class UpdatePortEndpoint
{
    public sealed record UpdatePortRequest(string Name, string? Code, bool IsActive);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/operations/ports/{id:guid}", async (
            Guid id,
            [FromBody] UpdatePortRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdatePortCommand(id, request.Name, request.Code, request.IsActive);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(new { success = true })
                : Results.BadRequest(new { error = result.Error });
        })
        .RequirePermission(PermissionCatalog.OperationsEdit);
    }
}
