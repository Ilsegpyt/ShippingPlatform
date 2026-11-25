namespace CustomsClearance.Domain.Entities;

// Rows exist only when the shipment's containers are split (mixed sizes).
public class CustomsContainer
{
    public Guid Id { get; set; }

    public Guid ShipmentId { get; set; }

    public short Ordinal { get; set; }

    public string ContainerType { get; set; } = null!; // 20, 40, 40 HC, 45 HC
}
