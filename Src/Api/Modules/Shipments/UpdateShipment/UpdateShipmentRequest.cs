using Shipments.Domain.Shipments;

namespace Api.Modules.Shipments.UpdateShipment;

public sealed class UpdateShipmentRequest
{
    public ShipmentStatus Status { get; set; }

    public string? MBL { get; set; }

    public string? HBL { get; set; }

    public string? MAWB { get; set; }

    public string? BookingConfirmationNumber { get; set; }
}