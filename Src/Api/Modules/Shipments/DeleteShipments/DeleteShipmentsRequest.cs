namespace Api.Modules.Shipments.DeleteShipments;

public sealed class DeleteShipmentsRequest
{
    public List<Guid> ShipmentIds { get; set; } = [];
}