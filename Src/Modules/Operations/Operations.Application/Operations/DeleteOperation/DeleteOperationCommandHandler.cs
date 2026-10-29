using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.DeleteOperation;

public sealed class DeleteOperationCommandHandler(
    IOperationRepository operationRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<DeleteOperationCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteOperationCommand command,
        CancellationToken ct)
    {
        var operation = await operationRepository.GetByIdForUpdateAsync(
            command.OperationId, ct);

        if (operation is null)
            return Result.Failure<bool>("Operation not found.");

        try
        {
            var userId = currentUser.UserId;
            var userName = await currentUser.GetUserNameAsync(ct);

            operation.MarkAsDeleted(
                userId,
                userName ?? "User",
                DateTime.UtcNow);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<bool>(ex.Message);
        }

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(true);
    }
}
