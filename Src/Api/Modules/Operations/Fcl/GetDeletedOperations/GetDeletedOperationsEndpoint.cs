using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.GetDeletedOperations;

namespace Api.Modules.Operations.GetDeletedOperations;

public static class GetDeletedOperationsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/operations/deleted", async (
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new GetDeletedOperationsQuery(), ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        }).RequirePermission(PermissionCatalog.OperationsView);
    }
}
