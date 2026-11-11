using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;

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

        var userId = currentUser.UserId != Guid.Empty ? currentUser.UserId : Guid.NewGuid();
        var userName = await currentUser.GetUserNameAsync(ct);
        var now = DateTime.UtcNow;

        if (operation.IsLockActive(now) && operation.LockedByUserId != userId)
        {
            return Result.Failure<LockFtlOperationResult>(
                $"Operation is currently locked by {operation.LockedByName}.");
        }

        operation.AcquireLock(userId, userName, now);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new LockFtlOperationResult(true, operation.LockedByName, operation.LockedAtUtc));
    }
}
