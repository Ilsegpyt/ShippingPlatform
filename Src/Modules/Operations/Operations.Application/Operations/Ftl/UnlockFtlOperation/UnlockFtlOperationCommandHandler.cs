using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.Ftl.UnlockFtlOperation;

public sealed class UnlockFtlOperationCommandHandler(
    IFtlOperationRepository operationRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<UnlockFtlOperationCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        UnlockFtlOperationCommand command,
        CancellationToken ct)
    {
        var operation = await operationRepository.GetByRefAsync(command.Ref, false, ct);
        if (operation is null)
            return Result.Failure<bool>($"Operation '{command.Ref}' not found.");

        operation.ReleaseLock();
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(true);
    }
}
