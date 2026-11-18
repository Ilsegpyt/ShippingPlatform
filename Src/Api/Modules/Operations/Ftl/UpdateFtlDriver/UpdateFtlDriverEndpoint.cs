using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.Ftl.UpdateFtlDriver;

namespace Api.Modules.Operations.Ftl.UpdateFtlDriver;

public static class UpdateFtlDriverEndpoint
{
    public sealed record UpdateFtlDriverRequest(string? DriverNumber, string Name, string? MobileNumber, bool IsActive);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/operations/ftl/drivers/{id:guid}", async (
            Guid id,
            [FromBody] UpdateFtlDriverRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateFtlDriverCommand(id, request.Name, request.MobileNumber, request.DriverNumber, request.IsActive);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(new { success = true })
                : Results.BadRequest(new { error = result.Error });
        })
        .RequirePermission(PermissionCatalog.FtlOperationsEdit);
    }
}
