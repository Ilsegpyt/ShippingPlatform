using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.UpdateDestination;

public sealed class UpdateDestinationCommandHandler(
    IDestinationRepository destinationRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<UpdateDestinationCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateDestinationCommand command, CancellationToken ct)
    {
        var dest = await destinationRepository.GetByIdAsync(command.Id, ct);
        if (dest is null) return Result.Failure<bool>("Destination not found.");

        var name = command.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name)) return Result.Failure<bool>("Destination name is required.");

        if (await destinationRepository.DestinationNameExistsAsync(name, command.Id, ct))
            return Result.Failure<bool>($"Destination '{name}' already exists.");

        dest.Update(name, command.IsActive);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success(true);
    }
}
