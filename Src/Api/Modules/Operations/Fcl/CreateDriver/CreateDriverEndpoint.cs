using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.CreateDriver;

namespace Api.Modules.Operations.CreateDriver;

public static class CreateDriverEndpoint
{
    public sealed record CreateDriverRequest(string DriverNumber, string Name, string? MobileNumber);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/operations/drivers", async (
            [FromBody] CreateDriverRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreateDriverCommand(request.DriverNumber, request.Name, request.MobileNumber);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(new { id = result.Value })
                : Results.BadRequest(new { error = result.Error });
        })
        .RequirePermission(PermissionCatalog.OperationsCreate);
    }
}
