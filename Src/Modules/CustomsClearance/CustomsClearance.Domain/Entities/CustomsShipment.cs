using CustomsClearance.Domain.Enums;

namespace CustomsClearance.Domain.Entities;

public class CustomsShipment
{
    public Guid Id { get; set; }

    public ReportType ReportType { get; set; }

    public string? ShipmentNumber { get; set; }

    public Guid ClientId { get; set; }

    public string? CertificateNumber { get; set; }

    public CustomsShipmentStatus Status { get; set; } = CustomsShipmentStatus.Open;

    public string? LatestUpdate { get; set; }

    public DateTime? ClosedAtUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public OceanImportDetails? OceanImportDetails { get; set; }
    public OceanExportDetails? OceanExportDetails { get; set; }
    public DomesticImportDetails? DomesticImportDetails { get; set; }

    public List<CustomsContainer> Containers { get; set; } = new();
    public List<CustomsUpdateLog> UpdateLogs { get; set; } = new();
}
