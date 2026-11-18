using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.Ftl.DeactivateFtlVehicle;

public sealed class DeactivateFtlVehicleCommandHandler(
    IFtlVehicleRepository vehicleRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<DeactivateFtlVehicleCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeactivateFtlVehicleCommand command, CancellationToken ct)
    {
        var vehicle = await vehicleRepository.GetByIdAsync(command.Id, ct);
        if (vehicle is null) return Result.Failure<bool>("FTL Vehicle not found.");

        var userName = await currentUser.GetUserNameAsync(ct);
        vehicle.MarkAsDeleted(currentUser.UserId, userName ?? "User", DateTime.UtcNow);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(true);
    }
}
