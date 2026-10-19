using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.ReopenOperation;

public sealed class ReopenOperationCommandHandler(
    IOperationRepository operationRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<ReopenOperationCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        ReopenOperationCommand command,
        CancellationToken ct)
    {
        var operation = await operationRepository.GetByIdForUpdateAsync(
            command.OperationId, ct);

        if (operation is null)
            return Result.Failure<bool>("Operation not found.");

        try
        {
            operation.Reopen(
                currentUser.UserId.ToString(),
                command.Reason,
                DateTime.UtcNow);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<bool>(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<bool>(ex.Message);
        }

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(true);
    }
}
