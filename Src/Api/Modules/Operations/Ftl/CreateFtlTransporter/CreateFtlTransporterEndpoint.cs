using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.Ftl.CreateFtlTransporter;

namespace Api.Modules.Operations.Ftl.CreateFtlTransporter;

public static class CreateFtlTransporterEndpoint
{
    public sealed record CreateFtlTransporterRequest(string Name);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/operations/ftl/transporters", async (
            [FromBody] CreateFtlTransporterRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreateFtlTransporterCommand(request.Name);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(new { id = result.Value })
                : Results.BadRequest(new { error = result.Error });
        })
        .RequirePermission(PermissionCatalog.FtlOperationsCreate);
    }
}
