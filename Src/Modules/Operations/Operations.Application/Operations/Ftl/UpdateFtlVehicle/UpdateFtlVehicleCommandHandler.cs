using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.Ftl.UpdateFtlVehicle;

public sealed class UpdateFtlVehicleCommandHandler(
    IFtlVehicleRepository vehicleRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<UpdateFtlVehicleCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateFtlVehicleCommand command, CancellationToken ct)
    {
        var vehicle = await vehicleRepository.GetByIdAsync(command.Id, ct);
        if (vehicle is null) return Result.Failure<bool>("FTL Vehicle not found.");

        var plate = command.PlateNumber?.Trim();
        if (string.IsNullOrWhiteSpace(plate))
            return Result.Failure<bool>("Plate number is required.");

        if (await vehicleRepository.PlateNumberExistsAsync(plate, command.Id, ct))
            return Result.Failure<bool>($"FTL Vehicle with plate '{plate}' already exists.");

        var truckType = FtlTruckTypeExtensions.ParseFtlTruckType(command.TruckType) ?? vehicle.TruckType;

        vehicle.Update(plate, truckType, command.FtlTransporterId, command.IsActive);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(true);
    }
}
