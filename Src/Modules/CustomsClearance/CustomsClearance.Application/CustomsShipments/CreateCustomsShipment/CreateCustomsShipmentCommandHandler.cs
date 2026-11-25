using BuildingBlocks.Application;
using CustomsClearance.Application.Abstractions;
using CustomsClearance.Application.Abstractions.Repositories;
using CustomsClearance.Domain.Entities;
using CustomsClearance.Domain.Enums;
using MediatR;

namespace CustomsClearance.Application.CustomsShipments.CreateCustomsShipment;

public sealed class CreateCustomsShipmentCommandHandler(
    ICustomsShipmentRepository customsShipmentRepository,
    ICustomsClearanceUnitOfWork customsClearanceUnitOfWork)
    : IRequestHandler<CreateCustomsShipmentCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateCustomsShipmentCommand cmd,
        CancellationToken ct)
    {
        var shipment = new CustomsShipment
        {
            ReportType = cmd.ReportType,
            ClientId = cmd.ClientId,
            Status = CustomsShipmentStatus.Open,
            CreatedAtUtc = DateTime.UtcNow
        };

        switch (cmd.ReportType)
        {
            case ReportType.OceanImport:
            {
                var d = cmd.OceanImportDetails!;
                var acidNumber = d.AcidNumber.Trim();
                var mbl = d.Mbl.Trim();

                if (await customsShipmentRepository.AcidNumberExistsAsync(acidNumber, ct))
                    return Result.Failure<Guid>("ACID # already exists in Ocean import.");

                if (await customsShipmentRepository.MblExistsAsync(mbl, ct))
                    return Result.Failure<Guid>("MB/L already exists in Ocean import.");

                shipment.OceanImportDetails = new OceanImportDetails
                {
                    ClearanceType = d.ClearanceType,
                    AcidNumber = acidNumber,
                    Mbl = mbl,
                    EtaDate = d.EtaDate
                };
                break;
            }

            case ReportType.OceanExport:
            {
                var d = cmd.OceanExportDetails!;
                var bookingNumber = d.BookingNumber.Trim();
                var commercialInvoiceNumber = d.CommercialInvoiceNumber.Trim();

                if (await customsShipmentRepository.BookingNumberExistsAsync(bookingNumber, ct))
                    return Result.Failure<Guid>("Booking number already exists in Ocean export.");

                if (await customsShipmentRepository.OceanExportCommercialInvoiceNumberExistsAsync(commercialInvoiceNumber, ct))
                    return Result.Failure<Guid>("Commercial invoice # already exists in Ocean export.");

                shipment.OceanExportDetails = new OceanExportDetails
                {
                    ClearanceType = d.ClearanceType,
                    Pol = d.Pol.Trim(),
                    Pod = d.Pod.Trim(),
                    BookingNumber = bookingNumber,
                    CommercialInvoiceNumber = commercialInvoiceNumber,
                    NumberOfContainers = d.NumberOfContainers
                };
                break;
            }

            case ReportType.Domestic:
            {
                var d = cmd.DomesticImportDetails!;
                var commercialInvoiceNumber = d.CommercialInvoiceNumber.Trim();

                if (await customsShipmentRepository.DomesticCommercialInvoiceNumberExistsAsync(commercialInvoiceNumber, ct))
                    return Result.Failure<Guid>("Commercial invoice # already exists in Domestic import.");

                shipment.DomesticImportDetails = new DomesticImportDetails
                {
                    CommercialInvoiceNumber = commercialInvoiceNumber,
                    NumberOfPieces = d.NumberOfPieces
                };
                break;
            }

            default:
                return Result.Failure<Guid>("Unknown report type.");
        }

        customsShipmentRepository.Add(shipment);

        await customsClearanceUnitOfWork.SaveChangesAsync(ct);

        return Result.Success(shipment.Id);
    }
}
