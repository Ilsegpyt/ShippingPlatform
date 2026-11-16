using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.CreateVehicle;

namespace Api.Modules.Operations.CreateVehicle;

public static class CreateVehicleEndpoint
{
    public sealed record CreateVehicleRequest(string CarNumber);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/operations/vehicles", async (
            [FromBody] CreateVehicleRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreateVehicleCommand(request.CarNumber);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(new { id = result.Value })
                : Results.BadRequest(new { error = result.Error });
        })
        .RequirePermission(PermissionCatalog.OperationsCreate);
    }
}
