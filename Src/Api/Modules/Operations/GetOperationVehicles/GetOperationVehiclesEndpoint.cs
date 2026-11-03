using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using Operations.Application.Abstractions;

namespace Api.Modules.Operations.GetOperationVehicles;

public static class GetOperationVehiclesEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/operations/vehicles", async (
            bool? includeInactive,
            IVehicleRepository repository,
            CancellationToken ct) =>
        {
            var items = await repository.GetAllAsync(includeInactive ?? false, ct);
            return Results.Ok(items.Select(x => new
            {
                id = x.Id,
                carNumber = x.CarNumber,
                isActive = x.IsActive && !x.IsDeleted,
                isDeleted = x.IsDeleted
            }));
        })
        .RequirePermission(PermissionCatalog.OperationsCreate);

        app.MapGet("/api/operations/vehicles/{id:guid}", async (
            Guid id,
            IVehicleRepository repository,
            CancellationToken ct) =>
        {
            var item = await repository.GetByIdAsync(id, ct);
            return item is not null
                ? Results.Ok(new { id = item.Id, carNumber = item.CarNumber, isActive = item.IsActive && !item.IsDeleted, isDeleted = item.IsDeleted })
                : Results.NotFound();
        })
        .RequirePermission(PermissionCatalog.OperationsCreate);
    }
}
