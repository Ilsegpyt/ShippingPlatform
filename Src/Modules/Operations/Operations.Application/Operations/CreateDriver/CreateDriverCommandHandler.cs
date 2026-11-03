using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Domain.Entities;

namespace Operations.Application.Operations.CreateDriver;

public sealed class CreateDriverCommandHandler(
    IDriverRepository driverRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<CreateDriverCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateDriverCommand command, CancellationToken ct)
    {
        var driverNum = command.DriverNumber?.Trim();
        var name = command.Name?.Trim();
        if (string.IsNullOrWhiteSpace(driverNum)) return Result.Failure<Guid>("Driver number is required.");
        if (string.IsNullOrWhiteSpace(name)) return Result.Failure<Guid>("Driver name is required.");

        if (await driverRepository.DriverNumberExistsAsync(driverNum, null, ct))
            return Result.Failure<Guid>($"Driver number '{driverNum}' already exists.");

        var driver = Driver.Create(driverNum, name, command.MobileNumber);
        await driverRepository.AddAsync(driver, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(driver.Id);
    }
}
