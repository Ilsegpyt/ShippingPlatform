using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.UpdateDriver;

namespace Api.Modules.Operations.UpdateDriver;

public static class UpdateDriverEndpoint
{
    public sealed record UpdateDriverRequest(string DriverNumber, string Name, string? MobileNumber, bool IsActive);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/operations/drivers/{id:guid}", async (
            Guid id,
            [FromBody] UpdateDriverRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateDriverCommand(id, request.DriverNumber, request.Name, request.MobileNumber, request.IsActive);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(new { success = true })
                : Results.BadRequest(new { error = result.Error });
        })
        .RequirePermission(PermissionCatalog.OperationsEdit);
    }
}
