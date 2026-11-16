using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using Operations.Application.Abstractions;

namespace Api.Modules.Operations.GetOperationDestinations;

public static class GetOperationDestinationsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/operations/destinations", async (
            int? page,
            int? pageSize,
            string? search,
            string? status,
            bool? includeInactive,
            IDestinationRepository repository,
            CancellationToken ct) =>
        {
            if (page.HasValue)
            {
                var p = Math.Max(1, page.Value);
                var ps = Math.Clamp(pageSize ?? 50, 1, 100);
                var effectiveStatus = status ?? (includeInactive == true ? "all" : "active");
                var result = await repository.GetPagedAsync(search, effectiveStatus, p, ps, ct);

                return Results.Ok(new
                {
                    items = result.Items.Select(x => new
                    {
                        id = x.Id,
                        name = x.Name,
                        isActive = x.IsActive && !x.IsDeleted,
                        isDeleted = x.IsDeleted
                    }),
                    totalCount = result.TotalCount,
                    activeCount = result.ActiveCount,
                    inactiveCount = result.InactiveCount,
                    page = p,
                    pageSize = ps,
                    totalPages = (int)Math.Ceiling((double)result.TotalCount / ps)
                });
            }

            var items = await repository.GetAllAsync(includeInactive ?? false, ct);
            return Results.Ok(items.Select(x => new
            {
                id = x.Id,
                name = x.Name,
                isActive = x.IsActive && !x.IsDeleted,
                isDeleted = x.IsDeleted
            }));
        })
        .RequirePermission(PermissionCatalog.OperationsCreate);

        app.MapGet("/api/operations/destinations/{id:guid}", async (
            Guid id,
            IDestinationRepository repository,
            CancellationToken ct) =>
        {
            var item = await repository.GetByIdAsync(id, ct);
            return item is not null
                ? Results.Ok(new { id = item.Id, name = item.Name, isActive = item.IsActive && !item.IsDeleted, isDeleted = item.IsDeleted })
                : Results.NotFound();
        })
        .RequirePermission(PermissionCatalog.OperationsCreate);
    }
}
