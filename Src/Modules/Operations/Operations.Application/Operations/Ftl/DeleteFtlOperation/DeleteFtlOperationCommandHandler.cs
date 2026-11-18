using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.Ftl.DeleteFtlOperation;

public sealed class DeleteFtlOperationCommandHandler(
    IFtlOperationRepository operationRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<DeleteFtlOperationCommand, Result<DeleteFtlOperationResult>>
{
    public async Task<Result<DeleteFtlOperationResult>> Handle(
        DeleteFtlOperationCommand command,
        CancellationToken ct)
    {
        var operation = await operationRepository.GetByRefAsync(command.Ref, false, ct);
        if (operation is null)
            return Result.Failure<DeleteFtlOperationResult>($"Operation '{command.Ref}' not found.");

        var currentUserGuid = currentUser.UserId != Guid.Empty ? currentUser.UserId : (Guid?)null;
        if (operation.IsLockActive(DateTime.UtcNow, 10))
        {
            if (currentUserGuid.HasValue && operation.LockedByUserId.HasValue && operation.LockedByUserId.Value != currentUserGuid.Value)
            {
                return Result.Failure<DeleteFtlOperationResult>(
                    $"Operation '{command.Ref}' is currently locked by {operation.LockedByName}. Delete halted.");
            }
        }

        var userId = currentUser.UserId != Guid.Empty ? currentUser.UserId : Guid.NewGuid();
        var userName = await currentUser.GetUserNameAsync(ct);
        var now = DateTime.UtcNow;

        operation.MarkAsDeleted(userId, userName, now);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new DeleteFtlOperationResult());
    }
}
