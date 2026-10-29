using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using Operations.Application.Abstractions;

namespace Api.Modules.Operations.GetOperationDestinations;

public static class GetOperationDestinationsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/operations/destinations", async (
            IOperationRepository repository,
            CancellationToken ct) =>
        {
            var items = await repository.GetActiveDestinationsAsync(ct);

            return Results.Ok(items.Select(x => new
            {
                id = x.Id,
                name = x.Name
            }));
        })
        .RequirePermission(PermissionCatalog.OperationsCreate);
    }
}
