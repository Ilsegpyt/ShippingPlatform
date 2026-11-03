using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Domain.Entities;

namespace Operations.Application.Operations.CreateVehicle;

public sealed class CreateVehicleCommandHandler(
    IVehicleRepository vehicleRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<CreateVehicleCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateVehicleCommand command, CancellationToken ct)
    {
        var carNum = command.CarNumber?.Trim();
        if (string.IsNullOrWhiteSpace(carNum))
            return Result.Failure<Guid>("Car number / plate is required.");

        if (await vehicleRepository.VehicleCarNumberExistsAsync(carNum, null, ct))
            return Result.Failure<Guid>($"Vehicle '{carNum}' already exists.");

        var vehicle = Vehicle.Create(carNum);
        await vehicleRepository.AddAsync(vehicle, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(vehicle.Id);
    }
}
