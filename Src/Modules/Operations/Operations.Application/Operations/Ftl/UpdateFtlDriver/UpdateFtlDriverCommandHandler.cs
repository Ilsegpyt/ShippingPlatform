using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.Ftl.UpdateFtlDriver;

public sealed class UpdateFtlDriverCommandHandler(
    IFtlDriverRepository driverRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<UpdateFtlDriverCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateFtlDriverCommand command, CancellationToken ct)
    {
        var driver = await driverRepository.GetByIdAsync(command.Id, ct);
        if (driver is null) return Result.Failure<bool>("FTL Driver not found.");

        var name = command.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<bool>("Driver name is required.");

        var driverNumber = command.DriverNumber?.Trim();
        var mobileNumber = command.MobileNumber?.Trim();

        if (await driverRepository.NameOrNumberExistsAsync(name, driverNumber, command.Id, ct))
            return Result.Failure<bool>($"FTL Driver '{name}' already exists or driver number is in use.");

        driver.Update(name, mobileNumber, driverNumber, command.IsActive);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(true);
    }
}
