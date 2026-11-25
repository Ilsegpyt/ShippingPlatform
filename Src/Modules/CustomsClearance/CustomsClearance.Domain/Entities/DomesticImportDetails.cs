namespace CustomsClearance.Domain.Entities;

public class DomesticImportDetails
{
    public Guid ShipmentId { get; set; }

    public string CommercialInvoiceNumber { get; set; } = null!;

    public int NumberOfPieces { get; set; }

    public string? MissingDocs { get; set; }

    public DateOnly? DateOfReceivingMissingDocs { get; set; }
}
