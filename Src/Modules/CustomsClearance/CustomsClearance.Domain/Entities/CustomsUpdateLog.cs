namespace CustomsClearance.Domain.Entities;

// Append-only history; CustomsShipment.LatestUpdate holds the newest text.
public class CustomsUpdateLog
{
    public Guid Id { get; set; }

    public Guid ShipmentId { get; set; }

    public string UpdateText { get; set; } = null!;

    public DateTime UpdatedAtUtc { get; set; }

    public string UpdatedBy { get; set; } = null!;
}
