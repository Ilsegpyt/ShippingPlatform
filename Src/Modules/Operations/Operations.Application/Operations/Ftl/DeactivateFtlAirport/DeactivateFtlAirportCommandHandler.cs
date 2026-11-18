using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.Ftl.DeactivateFtlAirport;

public sealed class DeactivateFtlAirportCommandHandler(
    IFtlAirportRepository airportRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<DeactivateFtlAirportCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeactivateFtlAirportCommand command, CancellationToken ct)
    {
        var airport = await airportRepository.GetByIdAsync(command.Id, ct);
        if (airport is null) return Result.Failure<bool>("FTL Airport not found.");

        var userName = await currentUser.GetUserNameAsync(ct);
        airport.MarkAsDeleted(currentUser.UserId, userName ?? "User", DateTime.UtcNow);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(true);
    }
}
