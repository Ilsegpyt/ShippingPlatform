using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.DeactivateDestination;

public sealed class DeactivateDestinationCommandHandler(
    IDestinationRepository destinationRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<DeactivateDestinationCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeactivateDestinationCommand command, CancellationToken ct)
    {
        var dest = await destinationRepository.GetByIdAsync(command.Id, ct);
        if (dest is null) return Result.Failure<bool>("Destination not found.");

        var userName = await currentUser.GetUserNameAsync(ct);
        dest.MarkAsDeleted(currentUser.UserId, userName ?? "User", DateTime.UtcNow);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(true);
    }
}
