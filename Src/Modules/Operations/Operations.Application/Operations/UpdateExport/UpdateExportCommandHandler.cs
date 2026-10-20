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
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
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

        var duplicateKey = OperationDuplicateCheckKey.Create(
            effectiveShipmentNumber,
            operation.ExportDetails.BookingNumber,
            effectiveCertificateNumber);

        if (duplicateKey is not null &&
            await operationRepository.DuplicateCheckKeyExistsAsync(
                OperationType.Export,
                duplicateKey,
                operation.Id,
                ct))
        {
            return Result.Failure<bool>(
                "This Export Shipment Number, Booking Number and Certificate Number combination already exists.");
        }

        operation.UpdateExportShipmentNumber(command.ShipmentNumber);
        operation.UpdateDuplicateCheckKey(duplicateKey);

        var userName = await currentUser.GetUserNameAsync(ct);
        var userId = currentUser.UserId.ToString();
        var utcNow = DateTime.UtcNow;

        foreach (var item in command.Containers)
        {
            var container = operation.Containers
                .First(x => x.Id == item.ContainerId);

            container.UpdateExportData(
                item.ContainerType,
                item.ContainerNumber,
                item.RequiredLoadingDate,
                item.RequiredLoadingTime,
                item.Destination);

            container.Updates.Add(ContainerUpdate.Create(
                container.Id,
                $"Export container updated. Container Number: {item.ContainerNumber ?? container.ContainerNumber ?? "N/A"}",
                userId,
                userName,
                utcNow));
        }

        operation.UpdateNumberOfContainers(
            operation.Containers.Count);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(true);
    }
}