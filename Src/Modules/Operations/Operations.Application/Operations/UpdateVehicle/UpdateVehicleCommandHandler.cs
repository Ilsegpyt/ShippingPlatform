using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.UpdateVehicle;

public sealed class UpdateVehicleCommandHandler(
    IVehicleRepository vehicleRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<UpdateVehicleCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateVehicleCommand command, CancellationToken ct)
    {
        var vehicle = await vehicleRepository.GetByIdAsync(command.Id, ct);
        if (vehicle is null) return Result.Failure<bool>("Vehicle not found.");

        var carNum = command.CarNumber?.Trim();
        if (string.IsNullOrWhiteSpace(carNum)) return Result.Failure<bool>("Car number is required.");

        if (await vehicleRepository.VehicleCarNumberExistsAsync(carNum, command.Id, ct))
            return Result.Failure<bool>($"Vehicle '{carNum}' already exists.");

        vehicle.Update(carNum, command.IsActive);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(true);
    }
}
