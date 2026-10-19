using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.DeliverOperation;

public sealed class DeliverOperationCommandHandler(
    IOperationRepository operationRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<DeliverOperationCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeliverOperationCommand command,
        CancellationToken ct)
    {
        var operation = await operationRepository.GetByIdForUpdateAsync(
            command.OperationId, ct);

        if (operation is null)
            return Result.Failure<bool>("Operation not found.");

        try
        {
            operation.MarkDelivered(currentUser.UserId.ToString(), DateTime.UtcNow);
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
