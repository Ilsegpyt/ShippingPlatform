using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.DeactivateShippingLine;

public sealed class DeactivateShippingLineCommandHandler(
    IShippingLineRepository shippingLineRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<DeactivateShippingLineCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeactivateShippingLineCommand command, CancellationToken ct)
    {
        var line = await shippingLineRepository.GetByIdAsync(command.Id, ct);
        if (line is null) return Result.Failure<bool>("Shipping line not found.");

        var userName = await currentUser.GetUserNameAsync(ct);
        line.MarkAsDeleted(currentUser.UserId, userName ?? "User", DateTime.UtcNow);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(true);
    }
}
