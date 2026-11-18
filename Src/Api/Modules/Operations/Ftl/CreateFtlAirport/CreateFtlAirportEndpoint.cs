using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.Ftl.CreateFtlAirport;

namespace Api.Modules.Operations.Ftl.CreateFtlAirport;

public static class CreateFtlAirportEndpoint
{
    public sealed record CreateFtlAirportRequest(string Name, string? Code);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/operations/ftl/airports", async (
            [FromBody] CreateFtlAirportRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreateFtlAirportCommand(request.Name, request.Code);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(new { id = result.Value })
                : Results.BadRequest(new { error = result.Error });
        })
        .RequirePermission(PermissionCatalog.FtlOperationsCreate);
    }
}
