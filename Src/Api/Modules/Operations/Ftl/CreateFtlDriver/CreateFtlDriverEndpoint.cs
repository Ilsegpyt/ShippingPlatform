using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.Ftl.CreateFtlDriver;

namespace Api.Modules.Operations.Ftl.CreateFtlDriver;

public static class CreateFtlDriverEndpoint
{
    public sealed record CreateFtlDriverRequest(string? DriverNumber, string Name, string? MobileNumber);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/operations/ftl/drivers", async (
            [FromBody] CreateFtlDriverRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreateFtlDriverCommand(request.Name, request.MobileNumber, request.DriverNumber);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(new { id = result.Value })
                : Results.BadRequest(new { error = result.Error });
        })
        .RequirePermission(PermissionCatalog.FtlOperationsCreate);
    }
}
