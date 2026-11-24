using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Application.Operations.Ftl.LockFtlOperation;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.Ftl.TakeOverFtlOperation;

public sealed class TakeOverFtlOperationCommandHandler(
    IFtlOperationRepository operationRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<TakeOverFtlOperationCommand, Result<LockFtlOperationResult>>
{
    public async Task<Result<LockFtlOperationResult>> Handle(
        TakeOverFtlOperationCommand command,
        CancellationToken ct)
    {
        var operation = await operationRepository.GetByRefAsync(command.Ref, false, ct);
        if (operation is null)
            return Result.Failure<LockFtlOperationResult>($"Operation '{command.Ref}' not found.");

        if (operation.Status == OperationStatus.Delivered)
            return Result.Failure<LockFtlOperationResult>("Delivered operations cannot be edited.");

        var userId = currentUser.UserId;
        if (userId == Guid.Empty)
            return Result.Failure<LockFtlOperationResult>("User authentication is required to take over operation.");

        var userName = await currentUser.GetUserNameAsync(ct);
        var now = DateTime.UtcNow;

        // Seize lock unconditionally for the authorized manager
        operation.AcquireLock(userId, userName ?? "Manager", now);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new LockFtlOperationResult(
            true,
            operation.LockedByName,
            operation.LockedAtUtc,
            operation.LockedByUserId));
    }
}
