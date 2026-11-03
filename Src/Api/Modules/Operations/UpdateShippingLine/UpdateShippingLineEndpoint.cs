using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Operations.Application.Operations.UpdateShippingLine;

namespace Api.Modules.Operations.UpdateShippingLine;

public static class UpdateShippingLineEndpoint
{
    public sealed record UpdateShippingLineRequest(string Name, string? Abbreviation, bool IsActive);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/operations/shipping-lines/{id:guid}", async (
            Guid id,
            [FromBody] UpdateShippingLineRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateShippingLineCommand(id, request.Name, request.Abbreviation, request.IsActive);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(new { success = true })
                : Results.BadRequest(new { error = result.Error });
        })
        .RequirePermission(PermissionCatalog.OperationsEdit);
    }
}
