using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.Ftl.UpdateFtlTransporter;

namespace Api.Modules.Operations.Ftl.UpdateFtlTransporter;

public static class UpdateFtlTransporterEndpoint
{
    public sealed record UpdateFtlTransporterRequest(string Name, bool IsActive);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/operations/ftl/transporters/{id:guid}", async (
            Guid id,
            [FromBody] UpdateFtlTransporterRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateFtlTransporterCommand(id, request.Name, request.IsActive);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(new { success = true })
                : Results.BadRequest(new { error = result.Error });
        })
        .RequirePermission(PermissionCatalog.FtlOperationsEdit);
    }
}
