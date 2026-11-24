namespace Transporters.Domain.Entities;

public class TransporterPortCoverage
{
    public Guid Id { get; set; }

    public Guid TransporterId { get; set; }

    public string PortName { get; set; } = null!;

    public int FreeHours { get; set; } = 24;
    public decimal DelayRatePerHour { get; set; } = 0;

    public string? OfficialReceiptNotes { get; set; }
    public string? GeneralNotes { get; set; }

    public string Status { get; set; } = "Draft"; // Draft, Approved, Rejected
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public List<TransporterLaneRate> LaneRates { get; set; } = new();
    public List<TransporterClientException> ClientExceptions { get; set; } = new();
}
