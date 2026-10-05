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
        var operation = new Operation(
            command.ClientId,
            command.ShippingLineId,
            command.OperationType,
            null,
            null,
            null);

        if (command.OperationType == OperationType.Import)
        {
            var importDetails = new ImportDetails(
                 command.MBLNumber!,
                 command.Destination!,
                 command.PODId!.Value,
                 command.FreeTimeTill!.Value,
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