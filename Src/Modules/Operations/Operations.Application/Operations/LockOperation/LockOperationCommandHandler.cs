using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.LockOperation;

public sealed class LockOperationCommandHandler(
    IOperationRepository operationRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<LockOperationCommand, Result<OperationLockDto>>
{
    public async Task<Result<OperationLockDto>> Handle(
        LockOperationCommand command,
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

        if (operation.IsLockActive(now, 10))
        {
            if (operation.LockedByUserId.HasValue && operation.LockedByUserId.Value != userId)
            {
                var lockerName = string.IsNullOrWhiteSpace(operation.LockedByName) ? "another user" : operation.LockedByName;
                return Result.Failure<OperationLockDto>($"Operation is currently locked by {lockerName}.");
            }
        }

        // Lock is free, expired, or held by current user -> acquire/renew
        operation.AcquireLock(userId, userName ?? "User", now);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new OperationLockDto(
            true,
            operation.LockedByUserId,
            operation.LockedByName,
            operation.LockedAtUtc));
    }
}

