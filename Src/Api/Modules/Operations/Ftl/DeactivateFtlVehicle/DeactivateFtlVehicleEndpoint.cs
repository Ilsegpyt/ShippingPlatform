using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.Ftl.DeactivateFtlVehicle;

namespace Api.Modules.Operations.Ftl.DeactivateFtlVehicle;

public static class DeactivateFtlVehicleEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/operations/ftl/vehicles/{id:guid}", async (
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new DeactivateFtlVehicleCommand(id);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(new { success = true })
                : Results.BadRequest(new { error = result.Error });
        })
        .RequirePermission(PermissionCatalog.FtlOperationsDelete);
    }
}
