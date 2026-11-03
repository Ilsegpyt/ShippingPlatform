using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.CreateTransporter;

namespace Api.Modules.Operations.CreateTransporter;

public static class CreateTransporterEndpoint
{
    public sealed record CreateTransporterRequest(string Name);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/operations/transporters", async (
            [FromBody] CreateTransporterRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreateTransporterCommand(request.Name);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(new { id = result.Value })
                : Results.BadRequest(new { error = result.Error });
        })
        .RequirePermission(PermissionCatalog.OperationsCreate);
    }
}
