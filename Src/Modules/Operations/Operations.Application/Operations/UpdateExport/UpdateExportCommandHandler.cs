using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Application.Common;
using Operations.Domain.Entities;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.UpdateExport;

public sealed class UpdateExportCommandHandler(
    IOperationRepository operationRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<UpdateExportCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        UpdateExportCommand command,
        CancellationToken ct)
    {
        var operation = await operationRepository
            .GetByIdForUpdateAsync(command.OperationId, ct);

        if (operation is null)
            return Result.Failure<bool>("Operation not found.");

        if (operation.OperationType != OperationType.Export ||
            operation.ExportDetails is null)
        {
            return Result.Failure<bool>(
                "This operation is not an Export operation.");
        }

        if (operation.Status == OperationStatus.Delivered)
        {
            return Result.Failure<bool>(
                "Delivered operations cannot be edited. Reopen the operation first.");
        }

        var duplicateContainer = command.Containers
            .GroupBy(x => x.ContainerId)
            .FirstOrDefault(g => g.Count() > 1);

        if (duplicateContainer is not null)
        {
            return Result.Failure<bool>(
                "A container can only appear once in the request.");
        }

        var containerIds = operation.Containers
            .Select(x => x.Id)
            .ToHashSet();

        var invalidContainer = command.Containers
            .FirstOrDefault(x => !containerIds.Contains(x.ContainerId));

        if (invalidContainer is not null)
        {
            return Result.Failure<bool>(
                $"Container '{invalidContainer.ContainerId}' does not belong to this operation.");
        }

        if (operation.ExportDetails.ClearanceType == ClearanceType.Cert)
        {
            if (!string.IsNullOrWhiteSpace(command.Customs))
            {
                return Result.Failure<bool>(
                    "Customs must be empty when Clearance Type is CERT.");
            }

            if (command.CertificateNumber is not null)
            {
                operation.UpdateExportCertificateNumber(
                    command.CertificateNumber);
            }

            operation.ExportDetails.UpdateCustoms(null);
        }
        else if (operation.ExportDetails.ClearanceType == ClearanceType.Bosla)
        {
            if (!string.IsNullOrWhiteSpace(command.CertificateNumber))
            {
                return Result.Failure<bool>(
                    "Certificate Number must be empty when Clearance Type is BOSLA.");
            }

            if (command.Customs is not null)
            {
                operation.ExportDetails.UpdateCustoms(command.Customs);
            }

            operation.UpdateExportCertificateNumber(null);
        }

        var effectiveShipmentNumber =
            command.ShipmentNumber ?? operation.ShipmentNumber;

        var effectiveCertificateNumber =
            operation.ExportDetails.ClearanceType == ClearanceType.Bosla
                ? null
                : command.CertificateNumber is not null
                    ? (string.IsNullOrWhiteSpace(command.CertificateNumber)
                        ? null
                        : command.CertificateNumber.Trim())
                    : operation.CertificateNumber;

        if (!string.IsNullOrWhiteSpace(effectiveShipmentNumber) &&
            await operationRepository.ShipmentNumberExistsAsync(effectiveShipmentNumber, operation.Id, ct))
        {
            return Result.Failure<bool>(
                $"رقم الشحنة (Shipment #) '{effectiveShipmentNumber}' مسجل بالفعل لعملية أخرى.");
        }

        if (operation.ExportDetails is not null &&
            !string.IsNullOrWhiteSpace(operation.ExportDetails.BookingNumber) &&
            await operationRepository.BookingNumberExistsAsync(operation.ExportDetails.BookingNumber, operation.Id, ct))
        {
            return Result.Failure<bool>(
                $"رقم البوكينج (Booking #) '{operation.ExportDetails.BookingNumber.Trim()}' مسجل بالفعل لشحنة صادر أخرى.");
        }

        operation.UpdateDuplicateCheckKey(null);

        var newlyAddedVehicles = new Dictionary<string, Vehicle>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in command.Containers)
        {
            if (item.TransporterId.HasValue &&
                !await operationRepository.ActiveTransporterExistsAsync(
                    item.TransporterId.Value, ct))
            {
                return Result.Failure<bool>(
                    $"Transporter '{item.TransporterId}' does not exist or is inactive.");
            }

            if (item.DriverId.HasValue &&
                !await operationRepository.ActiveDriverExistsAsync(
                    item.DriverId.Value, ct))
            {
                return Result.Failure<bool>(
                    $"Driver '{item.DriverId}' does not exist or is inactive.");
            }

            if (item.VehicleId.HasValue && string.IsNullOrWhiteSpace(item.CarNumber) &&
                !await operationRepository.ActiveVehicleExistsAsync(
                    item.VehicleId.Value, ct))
            {
                return Result.Failure<bool>(
                    $"Vehicle '{item.VehicleId}' does not exist or is inactive.");
            }
        }

        operation.UpdateExportShipmentNumber(command.ShipmentNumber);
        operation.UpdateDuplicateCheckKey(null);

        if (command.POLId.HasValue)
        {
            if (!await operationRepository.ActivePortExistsAsync(command.POLId.Value, ct))
                return Result.Failure<bool>($"POL '{command.POLId}' does not exist or is inactive.");
            operation.ExportDetails.UpdatePOL(command.POLId.Value);
        }

        if (command.PODId.HasValue)
        {
            if (!await operationRepository.ActivePortExistsAsync(command.PODId.Value, ct))
                return Result.Failure<bool>($"POD '{command.PODId}' does not exist or is inactive.");
            operation.ExportDetails.UpdatePOD(command.PODId.Value);
        }
        else if (command.ClearPOD)
        {
            operation.ExportDetails.UpdatePOD(null);
        }

        if (command.POWId.HasValue)
        {
            if (!await operationRepository.ActivePortExistsAsync(command.POWId.Value, ct))
                return Result.Failure<bool>($"POW '{command.POWId}' does not exist or is inactive.");
            operation.ExportDetails.UpdatePOW(command.POWId.Value);
        }
        else if (command.ClearPOW)
        {
            operation.ExportDetails.UpdatePOW(null);
        }

        operation.UpdateInvoicesReceivedDate(command.InvoicesReceivedDate);

        foreach (var item in command.Containers)
        {
            var container = operation.Containers
                .First(x => x.Id == item.ContainerId);

            Guid? resolvedVehicleId = item.VehicleId;

            if (!string.IsNullOrWhiteSpace(item.CarNumber))
            {
                var cleanCar = item.CarNumber.Trim();
                if (newlyAddedVehicles.TryGetValue(cleanCar, out var tracked))
                {
                    resolvedVehicleId = tracked.Id;
                }
                else
                {
                    var existingVehicle = await operationRepository.GetVehicleByCarNumberAsync(cleanCar, ct);
                    if (existingVehicle is not null)
                    {
                        resolvedVehicleId = existingVehicle.Id;
                    }
                    else
                    {
                        var newVehicle = Vehicle.Create(cleanCar);
                        await operationRepository.AddVehicleAsync(newVehicle, ct);
                        newlyAddedVehicles[cleanCar] = newVehicle;
                        resolvedVehicleId = newVehicle.Id;
                    }
                }
            }

            container.UpdateExportData(
                item.ContainerType,
                item.ContainerNumber,
                item.RequiredLoadingDate,
                item.RequiredLoadingTime,
                item.Destination,
                item.ActualArrivalDate,
                item.ActualArrivalTime,
                item.GateOutDate,
                item.GateOutTime,
                item.TransporterId,
                resolvedVehicleId,
                item.DriverId,
                item.MissingDocs,
                item.MissingDocsReceivedDate);
        }

        operation.UpdateNumberOfContainers(
            operation.Containers.Count);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(true);
    }
}