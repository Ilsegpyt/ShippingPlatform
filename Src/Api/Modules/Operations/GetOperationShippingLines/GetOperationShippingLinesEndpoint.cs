using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using Operations.Application.Abstractions;

namespace Api.Modules.Operations.GetOperationShippingLines;

public static class GetOperationShippingLinesEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/operations/shipping-lines", async (
            IOperationRepository repository,
            CancellationToken ct) =>
        {
            var items = await repository.GetActiveShippingLinesAsync(ct);

            return Results.Ok(items.Select(x => new
            {
                id = x.Id,
                name = x.Name
            }));
        })
        .RequirePermission(PermissionCatalog.OperationsCreate);
    }
}
