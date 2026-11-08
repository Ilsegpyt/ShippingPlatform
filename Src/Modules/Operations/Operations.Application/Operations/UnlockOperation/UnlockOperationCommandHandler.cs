using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.UnlockOperation;

public sealed class UnlockOperationCommandHandler(
    IOperationRepository operationRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<UnlockOperationCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        UnlockOperationCommand command,
        CancellationToken ct)
    {
        var operation = await operationRepository.GetByIdForUpdateAsync(
            command.OperationId, ct);

        if (operation is null)
            return Result.Failure<bool>("Operation not found.");

        var now = DateTime.UtcNow;
        var userId = currentUser.UserId;

        // If not locked or lock is already expired, consider it unlocked
        if (!operation.IsLockActive(now, 10))
        {
            if (operation.LockedByUserId.HasValue)
            {
                operation.ReleaseLock();
                await unitOfWork.SaveChangesAsync(ct);
            }
            return Result.Success(true);
        }

        // If held by current user or forced by caller
        if (operation.LockedByUserId == userId || command.Force)
        {
            operation.ReleaseLock();
            await unitOfWork.SaveChangesAsync(ct);
            return Result.Success(true);
        }

        return Result.Failure<bool>($"Cannot unlock operation locked by {operation.LockedByName}.");
    }
}

