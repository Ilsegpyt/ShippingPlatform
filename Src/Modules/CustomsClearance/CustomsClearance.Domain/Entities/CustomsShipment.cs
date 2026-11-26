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

    // Soft delete (Inactive archive). Status keeps its value (Open / Closed) from before deactivation.
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public string? DeletedByUserId { get; set; }
    public string? DeletedByName { get; set; }

    public void MarkAsInactive(string? userId, string? userName, DateTime atUtc)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Customs shipment is already inactive.");

        IsDeleted = true;
        DeletedAtUtc = atUtc;
        DeletedByUserId = userId;
        DeletedByName = string.IsNullOrWhiteSpace(userName) ? "User" : userName.Trim();
    }

    public OceanImportDetails? OceanImportDetails { get; set; }
    public OceanExportDetails? OceanExportDetails { get; set; }
    public DomesticImportDetails? DomesticImportDetails { get; set; }

    public List<CustomsContainer> Containers { get; set; } = new();
    public List<CustomsUpdateLog> UpdateLogs { get; set; } = new();
}
