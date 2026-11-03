using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.UpdateVehicle;

namespace Api.Modules.Operations.UpdateVehicle;

public static class UpdateVehicleEndpoint
{
    public sealed record UpdateVehicleRequest(string CarNumber, bool IsActive);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/operations/vehicles/{id:guid}", async (
            Guid id,
            [FromBody] UpdateVehicleRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateVehicleCommand(id, request.CarNumber, request.IsActive);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(new { success = true })
                : Results.BadRequest(new { error = result.Error });
        })
        .RequirePermission(PermissionCatalog.OperationsEdit);
    }
}
