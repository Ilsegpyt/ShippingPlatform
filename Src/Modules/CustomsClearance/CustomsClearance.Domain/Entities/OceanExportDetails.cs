using CustomsClearance.Domain.Enums;

namespace CustomsClearance.Domain.Entities;

public class OceanExportDetails
{
    public Guid ShipmentId { get; set; }

    public OceanExportClearanceType ClearanceType { get; set; }

    public string? CustomsArea { get; set; } // Only when Bosla

    public int NumberOfContainers { get; set; }
    public string? ContainerType { get; set; } // Merged type; null when split

    public string? Pol { get; set; }
    public string? Pod { get; set; }

    public string BookingNumber { get; set; } = null!;
    public string CommercialInvoiceNumber { get; set; } = null!;

    public DateTime? Cutoff { get; set; }

    public DateOnly? LoadingDate { get; set; }
    public DateOnly? CertificateDate { get; set; }
    public DateOnly? DateOfCoo { get; set; }
    public DateOnly? DateOfReceivingForm13 { get; set; }
}
