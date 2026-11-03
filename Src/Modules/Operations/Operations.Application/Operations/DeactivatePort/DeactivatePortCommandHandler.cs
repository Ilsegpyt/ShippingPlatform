using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.DeactivatePort;

public sealed class DeactivatePortCommandHandler(
    IPortRepository portRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<DeactivatePortCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeactivatePortCommand command, CancellationToken ct)
    {
        var port = await portRepository.GetByIdAsync(command.Id, ct);
        if (port is null) return Result.Failure<bool>("Port not found.");

        var userName = await currentUser.GetUserNameAsync(ct);
        port.MarkAsDeleted(currentUser.UserId, userName ?? "User", DateTime.UtcNow);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(true);
    }
}
