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

        if (command.Version.HasValue && command.Version.Value != operation.Version)
        {
            return Result.Failure<DeleteFtlOperationResult>(
                "Operation was modified by another user (409 Conflict).");
        }

        var userId = currentUser.UserId != Guid.Empty ? currentUser.UserId : Guid.NewGuid();
        var userName = await currentUser.GetUserNameAsync(ct);
        var now = DateTime.UtcNow;

        operation.MarkAsDeleted(userId, userName, now);
        operation.IncrementVersion();

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new DeleteFtlOperationResult(operation.Version));
    }
}
