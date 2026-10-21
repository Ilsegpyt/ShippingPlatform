
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
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
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

            if (item.VehicleId.HasValue &&
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

        var duplicateKey = OperationDuplicateCheckKey.Create(
            operation.ShipmentNumber,
            operation.ImportDetails?.MBLNumber,
            operation.CertificateNumber);

        if (duplicateKey is not null &&
            await operationRepository.DuplicateCheckKeyExistsAsync(
                OperationType.Import,
                duplicateKey,
                operation.Id,
                ct))
        {
            return Result.Failure<bool>(
                "This Import Shipment Number, M/BL Number and Certificate Number combination already exists.");
        }

        operation.UpdateDuplicateCheckKey(duplicateKey);

        var userName = await currentUser.GetUserNameAsync(ct);
        var userId = currentUser.UserId.ToString();
        var utcNow = DateTime.UtcNow;

        foreach (var item in command.Containers)
        {
            var container = operation.Containers
                .First(x => x.Id == item.ContainerId);

            container.UpdateImportData(
                item.ActualArrivalDate,
                item.ActualArrivalTime,
                item.GateOutDate,
                item.GateOutTime,
                item.ContainerType,
                item.ContainerNumber,
                item.TransporterId,
                item.VehicleId,
                item.DriverId,
                item.CheckOutFromThePort,
                item.MissingDocs,
                item.MissingDocsReceivedDate,
                item.Destination,
                item.FreeTimeTill);

            container.Updates.Add(ContainerUpdate.Create(
                container.Id,
                $"Import container updated. Container Number: {item.ContainerNumber ?? container.ContainerNumber ?? "N/A"}",
                userId,
                userName,
                utcNow));
        }

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(true);
    }
}
