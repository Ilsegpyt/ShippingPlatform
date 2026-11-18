using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.Ftl.UpdateFtlVehicle;

namespace Api.Modules.Operations.Ftl.UpdateFtlVehicle;

public static class UpdateFtlVehicleEndpoint
{
    public sealed record UpdateFtlVehicleRequest(string PlateNumber, string? TruckType, Guid? FtlTransporterId, bool IsActive);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/operations/ftl/vehicles/{id:guid}", async (
            Guid id,
            [FromBody] UpdateFtlVehicleRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateFtlVehicleCommand(id, request.PlateNumber, request.TruckType, request.FtlTransporterId, request.IsActive);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(new { success = true })
                : Results.BadRequest(new { error = result.Error });
        })
        .RequirePermission(PermissionCatalog.FtlOperationsEdit);
    }
}
