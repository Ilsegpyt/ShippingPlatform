using BuildingBlocks.Application;
using CustomsClearance.Domain.Enums;
using MediatR;

namespace CustomsClearance.Application.CustomsShipments.CreateCustomsShipment;

// Only the create-form fields. They lock after save (except OceanImport EtaDate).
// Exactly one details payload must match ReportType.
public sealed record CreateCustomsShipmentCommand(
    ReportType ReportType,
    Guid ClientId,
    CreateOceanImportDetails? OceanImportDetails,
    CreateOceanExportDetails? OceanExportDetails,
    CreateDomesticImportDetails? DomesticImportDetails)
    : IRequest<Result<Guid>>;

public sealed record CreateOceanImportDetails(
    OceanImportClearanceType ClearanceType,
    string AcidNumber,
    string Mbl,
    DateOnly EtaDate);

public sealed record CreateOceanExportDetails(
    OceanExportClearanceType ClearanceType,
    string Pol,
    string Pod,
    string BookingNumber,
    string CommercialInvoiceNumber,
    int NumberOfContainers);

public sealed record CreateDomesticImportDetails(
    string CommercialInvoiceNumber,
    int NumberOfPieces);
