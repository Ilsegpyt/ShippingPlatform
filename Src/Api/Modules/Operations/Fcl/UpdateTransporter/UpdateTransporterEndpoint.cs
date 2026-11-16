using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.UpdateTransporter;

namespace Api.Modules.Operations.UpdateTransporter;

public static class UpdateTransporterEndpoint
{
    public sealed record UpdateTransporterRequest(string Name, bool IsActive);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/operations/transporters/{id:guid}", async (
            Guid id,
            [FromBody] UpdateTransporterRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateTransporterCommand(id, request.Name, request.IsActive);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(new { success = true })
                : Results.BadRequest(new { error = result.Error });
        })
        .RequirePermission(PermissionCatalog.OperationsEdit);
    }
}
