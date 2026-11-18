using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.Ftl.DeactivateFtlDriver;

public sealed class DeactivateFtlDriverCommandHandler(
    IFtlDriverRepository driverRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<DeactivateFtlDriverCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeactivateFtlDriverCommand command, CancellationToken ct)
    {
        var driver = await driverRepository.GetByIdAsync(command.Id, ct);
        if (driver is null) return Result.Failure<bool>("FTL Driver not found.");

        var userName = await currentUser.GetUserNameAsync(ct);
        driver.MarkAsDeleted(currentUser.UserId, userName ?? "User", DateTime.UtcNow);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(true);
    }
}
