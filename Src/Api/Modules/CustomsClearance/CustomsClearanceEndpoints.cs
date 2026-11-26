using Api.Modules.CustomsClearance.CustomsShipments.AddCustomsUpdate;
using Api.Modules.CustomsClearance.CustomsShipments.CloseCustomsShipment;
using Api.Modules.CustomsClearance.CustomsShipments.CreateCustomsShipment;
using Api.Modules.CustomsClearance.CustomsShipments.GetCustomsAlertsSummary;
using Api.Modules.CustomsClearance.CustomsShipments.GetCustomsShipmentById;
using Api.Modules.CustomsClearance.CustomsShipments.GetCustomsLookups;
using Api.Modules.CustomsClearance.CustomsShipments.GetCustomsShipments;
using Api.Modules.CustomsClearance.CustomsShipments.GetCustomsUpdateLogs;
using Api.Modules.CustomsClearance.CustomsShipments.Inactive;
using Api.Modules.CustomsClearance.CustomsShipments.UpdateCustomsShipment;
using Api.Modules.CustomsClearance.Finance;

namespace Api.Modules.CustomsClearance;

// No hard delete and no Reopen by design. DELETE is a soft delete into the read-only Inactive archive.
public static class CustomsClearanceEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        GetCustomsShipmentsEndpoint.Map(app);
        GetCustomsShipmentByIdEndpoint.Map(app);
        CreateCustomsShipmentEndpoint.Map(app);
        UpdateCustomsShipmentEndpoint.Map(app);
        CloseCustomsShipmentEndpoint.Map(app);

        GetCustomsUpdateLogsEndpoint.Map(app);
        AddCustomsUpdateEndpoint.Map(app);

        GetCustomsLookupsEndpoint.Map(app);

        GetCustomsAlertsSummaryEndpoint.Map(app);

        CustomsInactiveEndpoints.Map(app);

        CustomsFinanceEndpoints.Map(app);
    }
}
