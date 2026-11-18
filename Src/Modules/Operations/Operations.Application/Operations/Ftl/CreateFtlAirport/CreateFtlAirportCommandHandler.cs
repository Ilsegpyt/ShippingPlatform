using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Domain.Entities.Ftl;

namespace Operations.Application.Operations.Ftl.CreateFtlAirport;

public sealed class CreateFtlAirportCommandHandler(
    IFtlAirportRepository airportRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<CreateFtlAirportCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateFtlAirportCommand command, CancellationToken ct)
    {
        var name = command.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Guid>("Airport name is required.");

        if (await airportRepository.NameExistsAsync(name, null, ct))
            return Result.Failure<Guid>($"FTL Airport '{name}' already exists.");

        var code = command.Code?.Trim();
        var airport = FtlAirport.Create(name, code);
        await airportRepository.AddAsync(airport, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(airport.Id);
    }
}
