using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.UpdateDriver;

public sealed class UpdateDriverCommandHandler(
    IDriverRepository driverRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<UpdateDriverCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateDriverCommand command, CancellationToken ct)
    {
        var driver = await driverRepository.GetByIdAsync(command.Id, ct);
        if (driver is null) return Result.Failure<bool>("Driver not found.");

        var driverNum = command.DriverNumber?.Trim();
        var name = command.Name?.Trim();
        if (string.IsNullOrWhiteSpace(driverNum)) return Result.Failure<bool>("Driver number is required.");
        if (string.IsNullOrWhiteSpace(name)) return Result.Failure<bool>("Driver name is required.");

        if (await driverRepository.DriverNumberExistsAsync(driverNum, command.Id, ct))
            return Result.Failure<bool>($"Driver number '{driverNum}' already exists.");

        driver.Update(driverNum, name, command.MobileNumber, command.IsActive);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(true);
    }
}
