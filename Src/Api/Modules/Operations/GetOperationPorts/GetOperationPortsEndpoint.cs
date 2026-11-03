using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using Operations.Application.Abstractions;

namespace Api.Modules.Operations.GetOperationPorts;

public static class GetOperationPortsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/operations/ports", async (
            bool? includeInactive,
            IPortRepository repository,
            CancellationToken ct) =>
        {
            var items = await repository.GetAllAsync(includeInactive ?? false, ct);
            return Results.Ok(items.Select(x => new
            {
                id = x.Id,
                name = x.Name,
                code = x.Code,
                isActive = x.IsActive && !x.IsDeleted,
                isDeleted = x.IsDeleted
            }));
        })
        .RequirePermission(PermissionCatalog.OperationsCreate);

        app.MapGet("/api/operations/ports/{id:guid}", async (
            Guid id,
            IPortRepository repository,
            CancellationToken ct) =>
        {
            var item = await repository.GetByIdAsync(id, ct);
            return item is not null
                ? Results.Ok(new { id = item.Id, name = item.Name, code = item.Code, isActive = item.IsActive && !item.IsDeleted, isDeleted = item.IsDeleted })
                : Results.NotFound();
        })
        .RequirePermission(PermissionCatalog.OperationsCreate);
    }
}
