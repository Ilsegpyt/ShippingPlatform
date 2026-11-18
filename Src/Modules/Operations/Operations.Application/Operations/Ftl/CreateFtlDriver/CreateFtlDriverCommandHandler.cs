using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Domain.Entities.Ftl;

namespace Operations.Application.Operations.Ftl.CreateFtlDriver;

public sealed class CreateFtlDriverCommandHandler(
    IFtlDriverRepository driverRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<CreateFtlDriverCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateFtlDriverCommand command, CancellationToken ct)
    {
        var name = command.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Guid>("Driver name is required.");

        var driverNumber = command.DriverNumber?.Trim();
        var mobileNumber = command.MobileNumber?.Trim();

        if (await driverRepository.NameOrNumberExistsAsync(name, driverNumber, null, ct))
            return Result.Failure<Guid>($"FTL Driver '{name}' already exists or driver number is in use.");

        var driver = FtlDriver.Create(name, mobileNumber, driverNumber);
        await driverRepository.AddAsync(driver, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(driver.Id);
    }
}
