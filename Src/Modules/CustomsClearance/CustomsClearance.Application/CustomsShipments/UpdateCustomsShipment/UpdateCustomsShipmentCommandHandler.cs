using BuildingBlocks.Application;
using CustomsClearance.Application.Abstractions;
using CustomsClearance.Application.Abstractions.Repositories;
using CustomsClearance.Domain.Entities;
using CustomsClearance.Domain.Enums;
using MediatR;

namespace CustomsClearance.Application.CustomsShipments.UpdateCustomsShipment;

public sealed class UpdateCustomsShipmentCommandHandler(
    ICustomsShipmentRepository customsShipmentRepository,
    ICustomsClearanceUnitOfWork customsClearanceUnitOfWork)
    : IRequestHandler<UpdateCustomsShipmentCommand, Result>
{
    public async Task<Result> Handle(
        UpdateCustomsShipmentCommand cmd,
        CancellationToken ct)
    {
        var shipment = await customsShipmentRepository.GetForUpdateAsync(cmd.Id, ct);

        if (shipment is null)
            return Result.Failure("Customs shipment was not found.");

        if (shipment.Status == CustomsShipmentStatus.Closed)
            return Result.Failure("Closed customs shipments are read-only.");

        // ---------- Header (table-editable) ----------
        var shipmentNumber = Clean(cmd.ShipmentNumber);
        if (shipmentNumber is not null &&
            await customsShipmentRepository.ShipmentNumberExistsAsync(shipment.ReportType, shipmentNumber, shipment.Id, ct))
            return Result.Failure("Shipment # already exists in this report.");

        var certificateNumber = Clean(cmd.CertificateNumber);
        if (certificateNumber is not null &&
            await customsShipmentRepository.CertificateNumberExistsAsync(shipment.ReportType, certificateNumber, shipment.Id, ct))
            return Result.Failure("Certificate # already exists in this report.");

        shipment.ShipmentNumber = shipmentNumber;
        shipment.CertificateNumber = certificateNumber;

        // ---------- Details ----------
        var detailsResult = shipment.ReportType switch
        {
            ReportType.OceanImport => await UpdateOceanImportAsync(shipment, cmd, ct),
            ReportType.OceanExport => UpdateOceanExport(shipment, cmd),
            ReportType.Domestic => UpdateDomesticImport(shipment, cmd),
            _ => Result.Failure("Unknown report type.")
        };

        if (detailsResult.IsFailure)
            return detailsResult;

        await customsClearanceUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }

    private async Task<Result> UpdateOceanImportAsync(
        CustomsShipment shipment,
        UpdateCustomsShipmentCommand cmd,
        CancellationToken ct)
    {
        if (cmd.OceanExportDetails is not null || cmd.DomesticImportDetails is not null)
            return Result.Failure("Details do not match the shipment report type.");

        var d = cmd.OceanImportDetails;
        if (d is null)
            return Result.Success();

        var details = shipment.OceanImportDetails!;

        var hbl = Clean(d.Hbl);
        if (hbl is not null &&
            await customsShipmentRepository.HblExistsAsync(hbl, shipment.Id, ct))
            return Result.Failure("HB/L already exists in Ocean import.");

        var containersResult = ApplyContainers(
            shipment,
            currentCount: details.NumberOfContainers,
            newCount: d.NumberOfContainers,
            d.ContainerType,
            d.Containers,
            out var containerType);

        if (containersResult.IsFailure)
            return containersResult;

        details.Hbl = hbl;
        details.CustomsDeclarations = Clean(d.CustomsDeclarations);
        details.NumberOfContainers = d.NumberOfContainers;
        details.ContainerType = containerType;
        details.ShippingLine = Clean(d.ShippingLine);
        details.Commodity = Clean(d.Commodity);
        details.Pod = Clean(d.Pod);
        details.Terminal = Clean(d.Terminal);
        details.EtaDate = d.EtaDate; // Only create-form field that stays editable
        details.AtaDate = d.AtaDate;
        details.FreeTime = d.FreeTime;
        details.DateOfFreeTimeEnd = d.DateOfFreeTimeEnd;
        details.DoDate = d.DoDate;
        details.MissingDocs = Clean(d.MissingDocs);
        details.InspectionDate = d.InspectionDate;
        details.CurrentStage = Clean(d.CurrentStage);

        return Result.Success();
    }

    private static Result UpdateOceanExport(
        CustomsShipment shipment,
        UpdateCustomsShipmentCommand cmd)
    {
        if (cmd.OceanImportDetails is not null || cmd.DomesticImportDetails is not null)
            return Result.Failure("Details do not match the shipment report type.");

        var d = cmd.OceanExportDetails;
        if (d is null)
            return Result.Success();

        var details = shipment.OceanExportDetails!;

        var customsArea = Clean(d.CustomsArea);
        if (details.ClearanceType == OceanExportClearanceType.Certificate && customsArea is not null)
            return Result.Failure("Customs area must be blank for Certificate.");

        // NumberOfContainers is a locked create-form field for Ocean export.
        var containersResult = ApplyContainers(
            shipment,
            currentCount: details.NumberOfContainers,
            newCount: details.NumberOfContainers,
            d.ContainerType,
            d.Containers,
            out var containerType);

        if (containersResult.IsFailure)
            return containersResult;

        details.CustomsArea = customsArea;
        details.ContainerType = containerType;
        details.Cutoff = d.Cutoff;
        details.LoadingDate = d.LoadingDate;
        details.CertificateDate = d.CertificateDate;
        details.DateOfCoo = d.DateOfCoo;
        details.DateOfReceivingForm13 = d.DateOfReceivingForm13;

        return Result.Success();
    }

    private static Result UpdateDomesticImport(
        CustomsShipment shipment,
        UpdateCustomsShipmentCommand cmd)
    {
        if (cmd.OceanImportDetails is not null || cmd.OceanExportDetails is not null)
            return Result.Failure("Details do not match the shipment report type.");

        var d = cmd.DomesticImportDetails;
        if (d is null)
            return Result.Success();

        var details = shipment.DomesticImportDetails!;

        details.MissingDocs = Clean(d.MissingDocs);
        details.DateOfReceivingMissingDocs = d.DateOfReceivingMissingDocs;

        return Result.Success();
    }

    // Merged: one shared ContainerType, no CustomsContainers rows.
    // Split: one CustomsContainers row per container, ContainerType null.
    private static Result ApplyContainers(
        CustomsShipment shipment,
        int? currentCount,
        int? newCount,
        string? containerType,
        IReadOnlyList<string>? containers,
        out string? mergedContainerType)
    {
        mergedContainerType = null;

        var isSplit = shipment.Containers.Count > 0;
        var wantsSplit = containers is { Count: > 0 };

        if (isSplit && newCount != currentCount)
            return Result.Failure("Merge the containers before changing the number of containers.");

        if (!wantsSplit)
        {
            mergedContainerType = Clean(containerType);
            shipment.Containers.Clear();
            return Result.Success();
        }

        if (newCount != containers!.Count)
            return Result.Failure("One container type is needed for every container.");

        var current = shipment.Containers
            .OrderBy(x => x.Ordinal)
            .Select(x => x.ContainerType);

        if (current.SequenceEqual(containers))
            return Result.Success();

        shipment.Containers.Clear();

        for (var i = 0; i < containers.Count; i++)
        {
            shipment.Containers.Add(new CustomsContainer
            {
                ShipmentId = shipment.Id,
                Ordinal = (short)(i + 1),
                ContainerType = containers[i]
            });
        }

        return Result.Success();
    }

    private static string? Clean(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
