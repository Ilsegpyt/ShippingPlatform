using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using Operations.Application.Abstractions;

namespace Api.Modules.Operations.GetOperationVehicles;

public static class GetOperationVehiclesEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/operations/vehicles", async (
            IOperationRepository repository,
            CancellationToken ct) =>
        {
            var items = await repository.GetActiveVehiclesAsync(ct);

            return Results.Ok(items.Select(x => new
            {
                id = x.Id,
                carNumber = x.CarNumber
            }));
        })
        .RequirePermission(PermissionCatalog.OperationsCreate);
    }
}
