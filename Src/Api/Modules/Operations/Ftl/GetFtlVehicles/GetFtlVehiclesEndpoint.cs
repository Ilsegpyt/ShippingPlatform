using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using Operations.Application.Abstractions;
using Operations.Domain.Enums;

namespace Api.Modules.Operations.Ftl.GetFtlVehicles;

public static class GetFtlVehiclesEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/operations/ftl/vehicles", async (
            int? page,
            int? pageSize,
            string? search,
            string? status,
            bool? includeInactive,
            IFtlVehicleRepository repository,
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
                        plateNumber = x.PlateNumber,
                        truckType = x.TruckType.ToFrontendString(),
                        truckTypeRaw = (int)x.TruckType,
                        ftlTransporterId = x.FtlTransporterId,
                        transporterName = x.Transporter?.Name,
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
                plateNumber = x.PlateNumber,
                truckType = x.TruckType.ToFrontendString(),
                truckTypeRaw = (int)x.TruckType,
                ftlTransporterId = x.FtlTransporterId,
                transporterName = x.Transporter?.Name,
                isActive = x.IsActive && !x.IsDeleted,
                isDeleted = x.IsDeleted
            }));
        })
        .RequirePermission(PermissionCatalog.FtlOperationsCreate);

        app.MapGet("/api/operations/ftl/vehicles/{id:guid}", async (
            Guid id,
            IFtlVehicleRepository repository,
            CancellationToken ct) =>
        {
            var item = await repository.GetByIdAsync(id, ct);
            return item is not null
                ? Results.Ok(new
                {
                    id = item.Id,
                    plateNumber = item.PlateNumber,
                    truckType = item.TruckType.ToFrontendString(),
                    truckTypeRaw = (int)item.TruckType,
                    ftlTransporterId = item.FtlTransporterId,
                    transporterName = item.Transporter?.Name,
                    isActive = item.IsActive && !item.IsDeleted,
                    isDeleted = item.IsDeleted
                })
                : Results.NotFound();
        })
        .RequirePermission(PermissionCatalog.FtlOperationsCreate);
    }
}
