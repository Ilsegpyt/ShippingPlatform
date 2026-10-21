using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using Operations.Application.Abstractions;

namespace Api.Modules.Operations.GetOperationDrivers;

public static class GetOperationDriversEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/operations/drivers", async (
            IOperationRepository repository,
            CancellationToken ct) =>
        {
            var items = await repository.GetActiveDriversAsync(ct);

            return Results.Ok(items.Select(x => new
            {
                id = x.Id,
                driverNumber = x.DriverNumber,
                name = x.Name,
                mobileNumber = x.MobileNumber
            }));
        })
        .RequirePermission(PermissionCatalog.OperationsCreate);
    }
}
