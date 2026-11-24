using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.Ftl.LockFtlOperation;

public sealed class LockFtlOperationCommandHandler(
    IFtlOperationRepository operationRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<LockFtlOperationCommand, Result<LockFtlOperationResult>>
{
    public async Task<Result<LockFtlOperationResult>> Handle(
        LockFtlOperationCommand command,
        CancellationToken ct)
    {
        var operation = await operationRepository.GetByRefAsync(command.Ref, false, ct);
        if (operation is null)
            return Result.Failure<LockFtlOperationResult>($"Operation '{command.Ref}' not found.");

        if (operation.Status == OperationStatus.Delivered)
            return Result.Failure<LockFtlOperationResult>("Delivered operations cannot be edited.");

        var userId = currentUser.UserId;
        if (userId == Guid.Empty)
            return Result.Failure<LockFtlOperationResult>("User authentication is required to lock operation.");

        var userName = await currentUser.GetUserNameAsync(ct);
        var now = DateTime.UtcNow;

        if (operation.IsLockActive(now, 10))
        {
            if (operation.LockedByUserId.HasValue && operation.LockedByUserId.Value != userId)
            {
                var lockerName = string.IsNullOrWhiteSpace(operation.LockedByName) ? "another user" : operation.LockedByName;
                return Result.Failure<LockFtlOperationResult>(
                    $"Operation is currently locked by {lockerName}.");
            }
        }

        operation.AcquireLock(userId, userName ?? "User", now);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new LockFtlOperationResult(true, operation.LockedByName, operation.LockedAtUtc, operation.LockedByUserId));
    }
}
