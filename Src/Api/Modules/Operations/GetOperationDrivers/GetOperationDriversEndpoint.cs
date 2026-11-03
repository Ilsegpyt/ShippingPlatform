using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using Operations.Application.Abstractions;

namespace Api.Modules.Operations.GetOperationDrivers;

public static class GetOperationDriversEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/operations/drivers", async (
            bool? includeInactive,
            IDriverRepository repository,
            CancellationToken ct) =>
        {
            var items = await repository.GetAllAsync(includeInactive ?? false, ct);
            return Results.Ok(items.Select(x => new
            {
                id = x.Id,
                driverNumber = x.DriverNumber,
                name = x.Name,
                mobileNumber = x.MobileNumber,
                isActive = x.IsActive && !x.IsDeleted,
                isDeleted = x.IsDeleted
            }));
        })
        .RequirePermission(PermissionCatalog.OperationsCreate);

        app.MapGet("/api/operations/drivers/{id:guid}", async (
            Guid id,
            IDriverRepository repository,
            CancellationToken ct) =>
        {
            var item = await repository.GetByIdAsync(id, ct);
            return item is not null
                ? Results.Ok(new { id = item.Id, driverNumber = item.DriverNumber, name = item.Name, mobileNumber = item.MobileNumber, isActive = item.IsActive && !item.IsDeleted, isDeleted = item.IsDeleted })
                : Results.NotFound();
        })
        .RequirePermission(PermissionCatalog.OperationsCreate);
    }
}
