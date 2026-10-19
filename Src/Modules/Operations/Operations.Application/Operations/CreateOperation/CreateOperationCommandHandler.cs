using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Domain.Entities;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.CreateOperation;

public sealed class CreateOperationCommandHandler(
    IOperationRepository operationRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<CreateOperationCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateOperationCommand command,
        CancellationToken ct)
    {
        if (command.NumberOfContainers is < 1 or > 200)
        {
            return Result.Failure<Guid>(
                "Number of containers must be between 1 and 200.");
        }

        if (command.OperationType == OperationType.Import)
        {
            if (string.IsNullOrWhiteSpace(command.Destination))
            {
                return Result.Failure<Guid>(
                    "Destination is required for Import.");
            }

            if (!command.FreeTimeTill.HasValue)
            {
                return Result.Failure<Guid>(
                    "Free time till is required for Import.");
            }
        }

        var operation = new Operation(
            command.ClientId,
            command.ShippingLineId,
            command.OperationType,
            null,
            null,
            null);

        operation.UpdateNumberOfContainers(command.NumberOfContainers);

        for (short ordinal = 1; ordinal <= command.NumberOfContainers; ordinal++)
        {
            var container = new OperationContainer(
                operation.Id,
                ordinal);

            if (command.OperationType == OperationType.Import)
            {
                container.InitializeImportData(
                    command.Destination!,
                    command.FreeTimeTill!.Value);
            }
            else if (command.OperationType == OperationType.Export &&
                     !string.IsNullOrWhiteSpace(command.ExportDestination))
            {
                container.UpdateExportData(
                    null,
                    null,
                    null,
                    null,
                    command.ExportDestination);
            }

            operation.Containers.Add(container);
        }

        if (command.OperationType == OperationType.Import)
        {
            var importDetails = new ImportDetails(
                command.MBLNumber!,
                command.PODId!.Value,
                command.RequiredOffloadingDate!.Value,
                command.RequiredOffloadingTime!.Value);

            operation.SetImportDetails(importDetails);
        }
        else if (command.OperationType == OperationType.Export)
        {
            var exportDetails = new ExportDetails(
                command.ClearanceType!.Value,
                command.BookingNumber!,
                command.POLId!.Value,
                command.POWId,
                command.CutOffDate);

            operation.SetExportDetails(exportDetails);
        }

        await operationRepository.AddAsync(operation, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(operation.Id);
    }
}