using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.Ftl.CreateFtlVehicle;

namespace Api.Modules.Operations.Ftl.CreateFtlVehicle;

public static class CreateFtlVehicleEndpoint
{
    public sealed record CreateFtlVehicleRequest(string PlateNumber, string? TruckType, Guid? FtlTransporterId);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/operations/ftl/vehicles", async (
            [FromBody] CreateFtlVehicleRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreateFtlVehicleCommand(request.PlateNumber, request.TruckType, request.FtlTransporterId);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(new { id = result.Value })
                : Results.BadRequest(new { error = result.Error });
        })
        .RequirePermission(PermissionCatalog.FtlOperationsCreate);
    }
}
