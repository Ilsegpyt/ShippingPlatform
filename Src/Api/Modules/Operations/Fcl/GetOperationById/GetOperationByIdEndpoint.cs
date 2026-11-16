using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.GetOperationById;

namespace Api.Modules.Operations.GetOperationById;

public static class GetOperationByIdEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/operations/{id:guid}", async (
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GetOperationByIdQuery(id), ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound(new { Error = result.Error });
        })
        .RequirePermission(PermissionCatalog.OperationsView);
    }
}
