using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.GetAllOperations;

namespace Api.Modules.Operations.GetAllOperations;

public static class GetAllOperationsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/operations", async (
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GetAllOperationsQuery(), ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new { Error = result.Error });
        })
        .RequirePermission(PermissionCatalog.OperationsView);
    }
}
