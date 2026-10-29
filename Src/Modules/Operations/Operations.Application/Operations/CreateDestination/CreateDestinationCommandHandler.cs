using BuildingBlocks.Application;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Domain.Entities;

namespace Operations.Application.Operations.CreateDestination;

public sealed class CreateDestinationCommandHandler(
    IOperationRepository operationRepository,
    IOperationsUnitOfWork unitOfWork)
    : IRequestHandler<CreateDestinationCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateDestinationCommand command,
        CancellationToken ct)
    {
        var trimmed = command.Name?.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
            return Result.Failure<Guid>("Destination name is required.");

        var exists = await operationRepository.ActiveDestinationNameExistsAsync(trimmed, ct);
        if (exists)
            return Result.Failure<Guid>($"Destination '{trimmed}' already exists.");

        var destination = Destination.Create(trimmed);

        await operationRepository.AddDestinationAsync(destination, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(destination.Id);
    }
}
