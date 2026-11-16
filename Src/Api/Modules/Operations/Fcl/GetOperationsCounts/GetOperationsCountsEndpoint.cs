using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using Operations.Application.Abstractions;

namespace Api.Modules.Operations.GetOperationsCounts;

public static class GetOperationsCountsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/operations/counts", async (
            IOperationRepository repository,
            CancellationToken ct) =>
        {
            var counts = await repository.GetCountsAsync(ct);
            return Results.Ok(counts);
        })
        .RequirePermission(PermissionCatalog.OperationsView);
    }
}

