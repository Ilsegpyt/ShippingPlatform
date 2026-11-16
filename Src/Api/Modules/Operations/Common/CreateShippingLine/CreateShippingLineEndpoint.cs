using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.CreateShippingLine;

namespace Api.Modules.Operations.CreateShippingLine;

public static class CreateShippingLineEndpoint
{
    public sealed record CreateShippingLineRequest(string Name, string? Abbreviation = null);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/operations/shipping-lines", async (
            [FromBody] CreateShippingLineRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreateShippingLineCommand(request.Name, request.Abbreviation);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(new { id = result.Value })
                : Results.BadRequest(new { error = result.Error });
        })
        .RequirePermission(PermissionCatalog.OperationsCreate);
    }
}
