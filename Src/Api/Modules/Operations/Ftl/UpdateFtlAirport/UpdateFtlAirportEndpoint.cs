using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.Ftl.UpdateFtlAirport;

namespace Api.Modules.Operations.Ftl.UpdateFtlAirport;

public static class UpdateFtlAirportEndpoint
{
    public sealed record UpdateFtlAirportRequest(string Name, string? Code, bool IsActive);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/operations/ftl/airports/{id:guid}", async (
            Guid id,
            [FromBody] UpdateFtlAirportRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateFtlAirportCommand(id, request.Name, request.Code, request.IsActive);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(new { success = true })
                : Results.BadRequest(new { error = result.Error });
        })
        .RequirePermission(PermissionCatalog.FtlOperationsEdit);
    }
}
