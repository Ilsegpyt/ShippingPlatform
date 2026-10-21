using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using Operations.Application.Abstractions;

namespace Api.Modules.Operations.GetOperationPorts;

public static class GetOperationPortsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/operations/ports", async (
            IOperationRepository repository,
            CancellationToken ct) =>
        {
            var items = await repository.GetActivePortsAsync(ct);

            return Results.Ok(items.Select(x => new
            {
                id = x.Id,
                name = x.Name,
                code = x.Code
            }));
        })
        .RequirePermission(PermissionCatalog.OperationsCreate);
    }
}
