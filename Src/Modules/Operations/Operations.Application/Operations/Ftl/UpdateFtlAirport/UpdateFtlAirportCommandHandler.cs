using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.Ftl.UpdateFtlAirport;

public sealed class UpdateFtlAirportCommandHandler(
    IFtlAirportRepository airportRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<UpdateFtlAirportCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateFtlAirportCommand command, CancellationToken ct)
    {
        var airport = await airportRepository.GetByIdAsync(command.Id, ct);
        if (airport is null) return Result.Failure<bool>("FTL Airport not found.");

        var name = command.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<bool>("Airport name is required.");

        if (await airportRepository.NameExistsAsync(name, command.Id, ct))
            return Result.Failure<bool>($"FTL Airport '{name}' already exists.");

        airport.Update(name, command.Code?.Trim(), command.IsActive);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(true);
    }
}
