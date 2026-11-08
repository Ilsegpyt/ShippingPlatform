using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Application.Operations.LockOperation;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.TakeOverOperation;

public sealed class TakeOverOperationCommandHandler(
    IOperationRepository operationRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<TakeOverOperationCommand, Result<OperationLockDto>>
{
    public async Task<Result<OperationLockDto>> Handle(
        TakeOverOperationCommand command,
        CancellationToken ct)
    {
        var operation = await operationRepository.GetByIdForUpdateAsync(
            command.OperationId, ct);

        if (operation is null)
            return Result.Failure<OperationLockDto>("Operation not found.");

        if (operation.Status == OperationStatus.Delivered)
            return Result.Failure<OperationLockDto>("Delivered operations cannot be edited.");

        var now = DateTime.UtcNow;
        var userId = currentUser.UserId;
        var userName = await currentUser.GetUserNameAsync(ct);

        // Seize lock unconditionally for the authorized manager
        operation.AcquireLock(userId, userName ?? "Manager", now);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new OperationLockDto(
            true,
            operation.LockedByUserId,
            operation.LockedByName,
            operation.LockedAtUtc));
    }
}

