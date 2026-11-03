
using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Application.Common;
using Operations.Domain.Entities;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.UpdateImport;

public sealed class UpdateImportCommandHandler(
    IOperationRepository operationRepository,
    IVehicleRepository vehicleRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<UpdateImportCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        UpdateImportCommand command,
        CancellationToken ct)
    {
        var operation =
            await operationRepository.GetByIdForUpdateAsync(
                command.OperationId,
                ct);

        if (operation is null)
            return Result.Failure<bool>("Operation not found.");

        if (operation.OperationType != OperationType.Import)
            return Result.Failure<bool>(
                "This operation is not an Import operation.");

        if (operation.Status == OperationStatus.Delivered)
            return Result.Failure<bool>(
                "Delivered operations cannot be edited. Reopen the operation first.");

        var duplicateContainer = command.Containers
            .GroupBy(x => x.ContainerId)
            .FirstOrDefault(g => g.Count() > 1);

        if (duplicateContainer is not null)
            return Result.Failure<bool>(
                "A container can only appear once in the request.");

        var operationContainerIds = operation.Containers
            .Select(x => x.Id)
            .ToHashSet();

        var invalidContainer = command.Containers
            .FirstOrDefault(
                x => !operationContainerIds.Contains(x.ContainerId));

        if (invalidContainer is not null)
            return Result.Failure<bool>(
                $"Container '{invalidContainer.ContainerId}' does not belong to this operation.");

        // Validate all referenced entities before modifying the operation.
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

        operation.UpdateImportData(
            command.ShipmentNumber,
            command.CertificateNumber,
            command.InvoicesReceivedDate);

        if (!string.IsNullOrWhiteSpace(command.ShipmentNumber) &&
            await operationRepository.ShipmentNumberExistsAsync(command.ShipmentNumber, operation.Id, ct))
        {
            return Result.Failure<bool>(
                $"رقم الشحنة (Shipment #) '{command.ShipmentNumber.Trim()}' مسجل بالفعل لعملية أخرى.");
        }

        if (operation.ImportDetails is not null &&
            !string.IsNullOrWhiteSpace(operation.ImportDetails.MBLNumber) &&
            await operationRepository.MblNumberExistsAsync(operation.ImportDetails.MBLNumber, operation.Id, ct))
        {
            return Result.Failure<bool>(
                $"رقم بوليصة الشحن (M/BL) '{operation.ImportDetails.MBLNumber.Trim()}' مسجل بالفعل لشحنة وارد أخرى.");
        }

        operation.UpdateDuplicateCheckKey(null);

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
                    var existingVehicle = await vehicleRepository.GetByCarNumberAsync(cleanCar, ct);
                    if (existingVehicle is not null)
                    {
                        resolvedVehicleId = existingVehicle.Id;
                    }
                    else
                    {
                        var newVehicle = Vehicle.Create(cleanCar);
                        await vehicleRepository.AddAsync(newVehicle, ct);
                        newlyAddedVehicles[cleanCar] = newVehicle;
                        resolvedVehicleId = newVehicle.Id;
                    }
                }
            }

            container.UpdateImportData(
                item.ActualArrivalDate,
                item.ActualArrivalTime,
                item.GateOutDate,
                item.GateOutTime,
                item.ContainerType,
                item.ContainerNumber,
                item.TransporterId,
                resolvedVehicleId,
                item.DriverId,
                item.CheckOutFromThePort,
                item.MissingDocs,
                item.MissingDocsReceivedDate,
                item.Destination,
                item.FreeTimeTill);
        }

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(true);
    }
}
