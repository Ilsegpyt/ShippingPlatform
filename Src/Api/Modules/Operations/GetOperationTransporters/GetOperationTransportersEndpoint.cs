using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using Operations.Application.Abstractions;

namespace Api.Modules.Operations.GetOperationTransporters;

public static class GetOperationTransportersEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/operations/transporters", async (
            IOperationRepository repository,
            CancellationToken ct) =>
        {
            var items = await repository.GetActiveTransportersAsync(ct);

            return Results.Ok(items.Select(x => new
            {
                id = x.Id,
                name = x.Name
            }));
        })
        .RequirePermission(PermissionCatalog.OperationsCreate);
    }
}
