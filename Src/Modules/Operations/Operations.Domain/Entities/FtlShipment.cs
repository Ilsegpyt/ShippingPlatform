using BuildingBlocks.Domain;
using Operations.Domain.Common;

namespace Operations.Domain.Entities;

public class FtlShipment : Entity<Guid>
{
    public Guid FtlOperationId { get; private set; }
    public FtlOperation Operation { get; private set; } = null!;

    public short Ordinal { get; private set; }
    public string? ShipmentNumber { get; private set; }
    public Guid? ClientId { get; private set; }
    public string? ClientName { get; private set; }
    public string? HAWB { get; private set; }
    public string? CertificateNumber { get; private set; }
    public int? Packages { get; private set; }
    public decimal? GrossWeight { get; private set; }
    public decimal? Cbm { get; private set; }
    public string? Commodity { get; private set; }
    public string? CustomsArea { get; private set; }
    public DateOnly? InvoicesReceivedDate { get; private set; }
    public string? TransporterInvoiceNo { get; private set; }
    public string? MissingDocs { get; private set; }
    public DateOnly? MissingDocsReceivedDate { get; private set; }

    private FtlShipment() { }

    public FtlShipment(
        Guid ftlOperationId,
        short ordinal,
        string? shipmentNumber = null,
        Guid? clientId = null,
        string? clientName = null,
        string? hawb = null,
        string? certificateNumber = null,
        int? packages = null,
        decimal? grossWeight = null,
        decimal? cbm = null,
        string? commodity = null,
        string? customsArea = null,
        DateOnly? invoicesReceivedDate = null,
        string? transporterInvoiceNo = null,
        string? missingDocs = null,
        DateOnly? missingDocsReceivedDate = null)
        : base(Guid.NewGuid())
    {
        if (ftlOperationId == Guid.Empty)
            throw new ArgumentException("Operation is required.", nameof(ftlOperationId));

        FtlOperationId = ftlOperationId;
        Ordinal = ordinal;
        ShipmentNumber = IdentifierNormalizer.Normalize(shipmentNumber);
        ClientId = clientId;
        ClientName = string.IsNullOrWhiteSpace(clientName) ? null : clientName.Trim();
        HAWB = IdentifierNormalizer.Normalize(hawb);
        CertificateNumber = IdentifierNormalizer.Normalize(certificateNumber);
        Packages = packages;
        GrossWeight = grossWeight;
        Cbm = cbm;
        Commodity = string.IsNullOrWhiteSpace(commodity) ? null : commodity.Trim();
        CustomsArea = string.IsNullOrWhiteSpace(customsArea) ? null : customsArea.Trim();
        InvoicesReceivedDate = invoicesReceivedDate;
        TransporterInvoiceNo = string.IsNullOrWhiteSpace(transporterInvoiceNo) ? null : transporterInvoiceNo.Trim();
        MissingDocs = string.IsNullOrWhiteSpace(missingDocs) ? null : missingDocs.Trim();
        MissingDocsReceivedDate = missingDocsReceivedDate;
    }

    public void Update(
        string? shipmentNumber,
        Guid? clientId,
        string? clientName,
        string? hawb,
        string? certificateNumber,
        int? packages,
        decimal? grossWeight,
        decimal? cbm,
        string? commodity,
        string? customsArea,
        DateOnly? invoicesReceivedDate,
        string? transporterInvoiceNo,
        string? missingDocs,
        DateOnly? missingDocsReceivedDate)
    {
        ShipmentNumber = IdentifierNormalizer.Normalize(shipmentNumber);
        ClientId = clientId;
        ClientName = string.IsNullOrWhiteSpace(clientName) ? null : clientName.Trim();
        HAWB = IdentifierNormalizer.Normalize(hawb);
        CertificateNumber = IdentifierNormalizer.Normalize(certificateNumber);
        Packages = packages;
        GrossWeight = grossWeight;
        Cbm = cbm;
        Commodity = string.IsNullOrWhiteSpace(commodity) ? null : commodity.Trim();
        CustomsArea = string.IsNullOrWhiteSpace(customsArea) ? null : customsArea.Trim();
        InvoicesReceivedDate = invoicesReceivedDate;
        TransporterInvoiceNo = string.IsNullOrWhiteSpace(transporterInvoiceNo) ? null : transporterInvoiceNo.Trim();
        MissingDocs = string.IsNullOrWhiteSpace(missingDocs) ? null : missingDocs.Trim();
        MissingDocsReceivedDate = missingDocsReceivedDate;
    }
}
