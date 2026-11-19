using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.Ftl.UpdateFtlStatus;

public sealed class UpdateFtlStatusCommandHandler(
    IFtlOperationRepository operationRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<UpdateFtlStatusCommand, Result<UpdateFtlStatusResult>>
{
    public async Task<Result<UpdateFtlStatusResult>> Handle(
        UpdateFtlStatusCommand command,
        CancellationToken ct)
    {
        var operation = await operationRepository.GetByRefAsync(command.Ref, true, ct);
        if (operation is null)
            return Result.Failure<UpdateFtlStatusResult>($"Operation '{command.Ref}' not found.");

        var currentUserGuid = currentUser.UserId != Guid.Empty ? currentUser.UserId : (Guid?)null;
        if (operation.IsLockActive(DateTime.UtcNow, 10))
        {
            if (currentUserGuid.HasValue && operation.LockedByUserId.HasValue && operation.LockedByUserId.Value != currentUserGuid.Value)
            {
                return Result.Failure<UpdateFtlStatusResult>(
                    $"Operation '{command.Ref}' is currently locked by {operation.LockedByName}. Status update halted.");
            }
        }

        if (!Enum.TryParse<OperationStatus>(command.Status, true, out var targetStatus))
            return Result.Failure<UpdateFtlStatusResult>($"Invalid status '{command.Status}'.");

        var userId = currentUser.UserId != Guid.Empty ? currentUser.UserId.ToString() : "u-1";
        var userName = await currentUser.GetUserNameAsync(ct);
        var now = DateTime.UtcNow;

        if (targetStatus == OperationStatus.Delivered)
        {
            if (operation.Status == OperationStatus.Open)
            {
                operation.MarkDelivered(userId, userName, now);
            }
            else
            {
                operation.UpdateStatus(targetStatus, userId, userName, now);
            }
        }
        else if (targetStatus == OperationStatus.Open && operation.Status == OperationStatus.Delivered)
        {
            operation.Reopen(userId, userName, "Reopened via status update", now);
        }
        else
        {
            operation.UpdateStatus(targetStatus, userId, userName, now);
        }

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new UpdateFtlStatusResult());
    }
}
