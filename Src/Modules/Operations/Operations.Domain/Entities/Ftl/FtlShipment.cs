using BuildingBlocks.Domain;
using Operations.Domain.Common;
using Operations.Domain.Enums;

namespace Operations.Domain.Entities.Ftl;

public class FtlShipment : Entity<Guid>
{
    public Guid FtlOperationId { get; private set; }
    public FtlOperation Operation { get; private set; } = null!;

    public short Ordinal { get; private set; }

    public string? ShipmentNumber { get; private set; }
    public string? MAWB { get; private set; }
    public string? HAWB { get; private set; }
    public string? CertificateNumber { get; private set; }

    public ClearanceType? ClearanceType { get; private set; }
    public Guid? ClientId { get; private set; }
    public string? ClientName { get; private set; }
    public string? CustomsArea { get; private set; }

    public int? Packages { get; private set; }
    public decimal? GrossWeight { get; private set; }
    public decimal? Cbm { get; private set; }
    public string? Commodity { get; private set; }

    public string? Terminal { get; private set; }
    public string? Destination { get; private set; }
    public string? FullAddress { get; private set; }
    public string? AOD { get; private set; }

    public DateOnly? InvoicesReceivedDate { get; private set; }
    public string? TransporterInvoiceNo { get; private set; }
    public string? MissingDocs { get; private set; }
    public DateOnly? MissingDocsReceivedDate { get; private set; }

    private FtlShipment() { }

    public FtlShipment(
        Guid ftlOperationId,
        short ordinal,
        string? shipmentNumber = null,
        string? mawb = null,
        string? hawb = null,
        string? certificateNumber = null,
        ClearanceType? clearanceType = null,
        Guid? clientId = null,
        string? clientName = null,
        string? customsArea = null,
        int? packages = null,
        decimal? grossWeight = null,
        decimal? cbm = null,
        string? commodity = null,
        string? terminal = null,
        string? destination = null,
        string? fullAddress = null,
        string? aod = null,
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
        Update(
            shipmentNumber,
            mawb,
            hawb,
            certificateNumber,
            clearanceType,
            clientId,
            clientName,
            customsArea,
            packages,
            grossWeight,
            cbm,
            commodity,
            terminal,
            destination,
            fullAddress,
            aod,
            invoicesReceivedDate,
            transporterInvoiceNo,
            missingDocs,
            missingDocsReceivedDate);
    }

    public void Update(
        string? shipmentNumber,
        string? mawb,
        string? hawb,
        string? certificateNumber,
        ClearanceType? clearanceType,
        Guid? clientId,
        string? clientName,
        string? customsArea,
        int? packages,
        decimal? grossWeight,
        decimal? cbm,
        string? commodity,
        string? terminal,
        string? destination,
        string? fullAddress,
        string? aod,
        DateOnly? invoicesReceivedDate,
        string? transporterInvoiceNo,
        string? missingDocs,
        DateOnly? missingDocsReceivedDate)
    {
        ShipmentNumber = IdentifierNormalizer.Normalize(shipmentNumber);
        MAWB = IdentifierNormalizer.Normalize(mawb);
        HAWB = IdentifierNormalizer.Normalize(hawb);
        CertificateNumber = IdentifierNormalizer.Normalize(certificateNumber);
        ClearanceType = clearanceType;
        ClientId = clientId;
        ClientName = string.IsNullOrWhiteSpace(clientName) ? null : clientName.Trim();
        CustomsArea = string.IsNullOrWhiteSpace(customsArea) ? null : customsArea.Trim();
        Packages = packages;
        GrossWeight = grossWeight;
        Cbm = cbm;
        Commodity = string.IsNullOrWhiteSpace(commodity) ? null : commodity.Trim();
        Terminal = string.IsNullOrWhiteSpace(terminal) ? null : terminal.Trim();
        Destination = string.IsNullOrWhiteSpace(destination) ? null : destination.Trim();
        FullAddress = string.IsNullOrWhiteSpace(fullAddress) ? null : fullAddress.Trim();
        AOD = string.IsNullOrWhiteSpace(aod) ? null : aod.Trim();
        InvoicesReceivedDate = invoicesReceivedDate;
        TransporterInvoiceNo = IdentifierNormalizer.Normalize(transporterInvoiceNo);
        MissingDocs = string.IsNullOrWhiteSpace(missingDocs) ? null : missingDocs.Trim();
        MissingDocsReceivedDate = missingDocsReceivedDate;
    }

    public void SetOrdinal(short ordinal)
    {
        Ordinal = ordinal;
    }
}
