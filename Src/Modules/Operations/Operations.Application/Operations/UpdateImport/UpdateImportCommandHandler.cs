using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;
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
        {
            return Result.Failure<bool>(
                "Operation not found.");
        }

        if (operation.OperationType != OperationType.Import)
        {
            return Result.Failure<bool>(
                "This operation is not an Import operation.");
        }

        operation.UpdateImportData(
            command.ShipmentNumber,
            command.CertificateNumber,
            command.InvoicesReceivedDate,
            command.Status);

        var userName = await currentUser.GetUserNameAsync(ct);
        var userId = currentUser.UserId.ToString();
        var utcNow = DateTime.UtcNow;

        var operationContainerIds =
            operation.Containers
                .Select(x => x.Id)
                .ToHashSet();

        foreach (var item in command.Containers)
        {
            if (!operationContainerIds.Contains(item.ContainerId))
            {
                return Result.Failure<bool>(
                    $"Container '{item.ContainerId}' does not belong to this operation.");
            }

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
                item.MissingDocsReceivedDate);

            var details =
                $"Import container updated. " +
                $"Container Number: {item.ContainerNumber ?? "N/A"}";

            var update = ContainerUpdate.Create(
                container.Id,
                details,
                userId,
                userName,
                utcNow);

            container.Updates.Add(update);
        }

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(true);
    }
}