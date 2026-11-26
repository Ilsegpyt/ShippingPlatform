using BuildingBlocks.Application;
using MediatR;

namespace CustomsClearance.Application.CustomsShipments.UpdateCustomsShipment;

// Table-editable fields only. Create-form fields (ClientId, ReportType, ClearanceType,
// AcidNumber, Mbl, Pol, Pod (export), BookingNumber, CommercialInvoiceNumber,
// NumberOfContainers (export), NumberOfPieces) are not here, so they cannot change.
// Exception: OceanImport EtaDate stays editable.
// Exactly one details payload, matching the shipment's ReportType, may be sent.
public sealed record UpdateCustomsShipmentCommand(
    Guid Id,
    string? ShipmentNumber,
    string? CertificateNumber,
    UpdateOceanImportDetails? OceanImportDetails,
    UpdateOceanExportDetails? OceanExportDetails,
    UpdateDomesticImportDetails? DomesticImportDetails)
    : IRequest<Result>;

// Containers: empty/null = merged (ContainerType holds the shared type).
// Containers with values = split; one type per container, in ordinal order.
public sealed record UpdateOceanImportDetails(
    string? Hbl,
    string? CustomsDeclarations,
    int? NumberOfContainers,
    string? ContainerType,
    IReadOnlyList<string>? Containers,
    string? ShippingLine,
    string? Commodity,
    string? Pod,
    string? Terminal,
    DateOnly EtaDate,
    DateOnly? AtaDate,
    int? FreeTime,
    DateOnly? DateOfFreeTimeEnd,
    DateOnly? DoDate,
    string? MissingDocs,
    string? Track,
    DateOnly? InspectionDate,
    string? CurrentStage);

public sealed record UpdateOceanExportDetails(
    string? CustomsArea,
    string? ContainerType,
    IReadOnlyList<string>? Containers,
    DateTime? Cutoff,
    DateOnly? LoadingDate,
    DateOnly? CertificateDate,
    DateOnly? DateOfCoo,
    DateOnly? DateOfReceivingForm13);

public sealed record UpdateDomesticImportDetails(
    string? MissingDocs,
    DateOnly? DateOfReceivingMissingDocs);
