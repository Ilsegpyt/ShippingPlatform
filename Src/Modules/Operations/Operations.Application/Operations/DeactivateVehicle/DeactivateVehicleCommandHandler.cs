using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.DeactivateVehicle;

public sealed class DeactivateVehicleCommandHandler(
    IVehicleRepository vehicleRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<DeactivateVehicleCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeactivateVehicleCommand command, CancellationToken ct)
    {
        var vehicle = await vehicleRepository.GetByIdAsync(command.Id, ct);
        if (vehicle is null) return Result.Failure<bool>("Vehicle not found.");

        var userName = await currentUser.GetUserNameAsync(ct);
        vehicle.MarkAsDeleted(currentUser.UserId, userName ?? "User", DateTime.UtcNow);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(true);
    }
}
