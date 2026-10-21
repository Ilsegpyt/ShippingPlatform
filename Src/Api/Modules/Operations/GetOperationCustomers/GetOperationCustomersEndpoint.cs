using Customers.Contracts;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;

namespace Api.Modules.Operations.GetOperationCustomers;

public static class GetOperationCustomersEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/operations/customers", async (
            ICustomerQueries customerQueries,
            CancellationToken ct) =>
        {
            var customers = await customerQueries.GetAllForOperationsAsync(ct);
            return Results.Ok(customers);
        })
        .RequirePermission(PermissionCatalog.OperationsCreate);
    }
}
