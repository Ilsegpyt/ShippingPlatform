using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Domain.Entities.Ftl;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.Ftl.CreateFtlVehicle;

public sealed class CreateFtlVehicleCommandHandler(
    IFtlVehicleRepository vehicleRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<CreateFtlVehicleCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateFtlVehicleCommand command, CancellationToken ct)
    {
        var plate = command.PlateNumber?.Trim();
        if (string.IsNullOrWhiteSpace(plate))
            return Result.Failure<Guid>("Plate number is required.");

        if (await vehicleRepository.PlateNumberExistsAsync(plate, null, ct))
            return Result.Failure<Guid>($"FTL Vehicle with plate '{plate}' already exists.");

        var truckType = FtlTruckTypeExtensions.ParseFtlTruckType(command.TruckType) ?? FtlTruckType.Jambo6m;

        var vehicle = FtlVehicle.Create(plate, truckType, command.FtlTransporterId);
        await vehicleRepository.AddAsync(vehicle, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(vehicle.Id);
    }
}
