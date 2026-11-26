using CustomsClearance.Domain.Enums;

namespace CustomsClearance.Domain.Entities;

public class OceanImportDetails
{
    public Guid ShipmentId { get; set; }

    public OceanImportClearanceType ClearanceType { get; set; }

    public string? AcidNumber { get; set; }
    public string Mbl { get; set; } = null!;
    public string? Hbl { get; set; }

    public string? CustomsDeclarations { get; set; }

    public int? NumberOfContainers { get; set; } // Entered in the table, not the create form
    public string? ContainerType { get; set; } // Merged type; null when split

    public string? ShippingLine { get; set; }
    public string? Commodity { get; set; }
    public string? Pod { get; set; }
    public string? Terminal { get; set; }

    public DateOnly EtaDate { get; set; }
    public DateOnly? AtaDate { get; set; }

    public int? FreeTime { get; set; }
    public DateOnly? DateOfFreeTimeEnd { get; set; }

    public DateOnly? DoDate { get; set; }

    public string? MissingDocs { get; set; }

    public string? Track { get; set; } // OceanImportTrack value; set from the table only

    public DateOnly? InspectionDate { get; set; }

    public string? CurrentStage { get; set; }
}
