using Customers.Contracts;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using Operations.Application.Abstractions;

namespace Api.Modules.CustomsClearance.CustomsShipments.GetCustomsLookups;

// Read-only dropdown sources for the Customs Clearance screens, under the customs view permission,
// so customs users do not need Operations or Customers permissions. Same projection style as
// /api/operations/ports and /api/operations/customers.
public static class GetCustomsLookupsEndpoint
{
    private static readonly string[] ContainerTypes =
    [
        "DryStandard20",
        "DryStandard40",
        "DryHigh40",
        "DryHigh45",
        "ReeferStandard20",
        "ReeferHigh40",
        "OpenTop20",
        "OpenTop40",
        "OpenTopHigh40",
        "FlatStandard40",
        "FlatHigh40",
        "Flat20",
        "Tank20",
        "Tank40"
    ];
    private static readonly string[] ClearanceTypes = ["Certificate", "Bosla", "Import", "Transit"];

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/customs-clearance/lookups",
            async (
                ICustomerQueries customerQueries,
                IPortRepository portRepository,
                IShippingLineRepository shippingLineRepository,
                CancellationToken ct) =>
            {
                var customers = await customerQueries.GetAllForOperationsAsync(ct);
                var ports = await portRepository.GetAllAsync(false, ct);
                var shippingLines = await shippingLineRepository.GetAllAsync(false, ct);

                return Results.Ok(new
                {
                    clients = customers
                        .Select(x => new { id = x.CustomerId, name = x.CompanyName })
                        .OrderBy(x => x.name),
                    ports = ports
                        .Select(x => new { id = x.Id, name = x.Name, code = x.Code })
                        .OrderBy(x => x.name),
                    shippingLines = shippingLines
                        .Select(x => new { id = x.Id, name = x.Name, abbreviation = x.Abbreviation })
                        .OrderBy(x => x.name),
                    // FTL terminals are not sea-port terminals, so customs has no terminal source yet.
                    terminals = Array.Empty<object>(),
                    containerTypes = ContainerTypes,
                    clearanceTypes = ClearanceTypes
                });
            }).RequirePermission(PermissionCatalog.CustomsShipmentsView);
    }
}
