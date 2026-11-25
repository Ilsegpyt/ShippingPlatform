using Api.Modules.CustomsClearance.CustomsShipments.AddCustomsUpdate;
using Api.Modules.CustomsClearance.CustomsShipments.CloseCustomsShipment;
using Api.Modules.CustomsClearance.CustomsShipments.CreateCustomsShipment;
using Api.Modules.CustomsClearance.CustomsShipments.GetCustomsShipmentById;
using Api.Modules.CustomsClearance.CustomsShipments.GetCustomsLookups;
using Api.Modules.CustomsClearance.CustomsShipments.GetCustomsShipments;
using Api.Modules.CustomsClearance.CustomsShipments.GetCustomsUpdateLogs;
using Api.Modules.CustomsClearance.CustomsShipments.UpdateCustomsShipment;

namespace Api.Modules.CustomsClearance;

// No Delete or Reopen endpoints by design.
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
    }
}
