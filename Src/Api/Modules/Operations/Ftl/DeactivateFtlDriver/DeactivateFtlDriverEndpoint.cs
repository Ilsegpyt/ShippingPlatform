using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.Ftl.DeactivateFtlDriver;

namespace Api.Modules.Operations.Ftl.DeactivateFtlDriver;

public static class DeactivateFtlDriverEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/operations/ftl/drivers/{id:guid}", async (
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new DeactivateFtlDriverCommand(id);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(new { success = true })
                : Results.BadRequest(new { error = result.Error });
        })
        .RequirePermission(PermissionCatalog.FtlOperationsDelete);
    }
}
