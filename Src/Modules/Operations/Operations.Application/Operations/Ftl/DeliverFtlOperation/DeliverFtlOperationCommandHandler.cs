using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.Ftl.DeliverFtlOperation;

public sealed class DeliverFtlOperationCommandHandler(
    IFtlOperationRepository repository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<DeliverFtlOperationCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeliverFtlOperationCommand command,
        CancellationToken ct)
    {
        var op = await repository.GetByRefAsync(command.Reference, true, ct);
        if (op is null && Guid.TryParse(command.Reference, out var guidId))
        {
            op = await repository.GetByIdForUpdateAsync(guidId, ct);
        }

        if (op is null)
            return Result.Failure<bool>($"FTL Operation '{command.Reference}' not found.");

        var userId = currentUser.UserId != Guid.Empty ? currentUser.UserId.ToString() : "u-1";
        var userName = await currentUser.GetUserNameAsync(ct);
        var now = DateTime.UtcNow;

        try
        {
            op.MarkDelivered(userId, userName, now);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>(ex.Message);
        }

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(true);
    }
}
