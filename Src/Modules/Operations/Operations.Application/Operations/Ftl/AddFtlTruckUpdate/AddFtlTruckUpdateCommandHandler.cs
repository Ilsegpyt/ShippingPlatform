using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;

namespace Operations.Application.Operations.Ftl.AddFtlTruckUpdate;

public sealed class AddFtlTruckUpdateCommandHandler(
    IFtlOperationRepository operationRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<AddFtlTruckUpdateCommand, Result<AddFtlTruckUpdateResult>>
{
    public async Task<Result<AddFtlTruckUpdateResult>> Handle(
        AddFtlTruckUpdateCommand command,
        CancellationToken ct)
    {
        var operation = await operationRepository.GetByRefAsync(command.Ref, true, ct);
        if (operation is null)
            return Result.Failure<AddFtlTruckUpdateResult>($"Operation '{command.Ref}' not found.");

        var userId = currentUser.UserId != Guid.Empty ? currentUser.UserId.ToString() : "u-1";
        var userName = await currentUser.GetUserNameAsync(ct);
        var now = DateTime.UtcNow;

        // Try to match truck from trip or first truck
        var trip = operation.Trips.FirstOrDefault(t => t.ExternalId == command.TripId);
        var truck = trip?.Truck ?? operation.Trucks.FirstOrDefault();

        if (truck is null)
        {
            // Create a default truck if none exists yet
            truck = new Domain.Entities.FtlTruck(operation.Id, 1);
            operation.Trucks.Add(truck);
        }

        var update = truck.AddUpdate(command.TripId, command.Note, userId, userName, now);
        operation.IncrementVersion();

        await unitOfWork.SaveChangesAsync(ct);

        var result = new AddFtlTruckUpdateResult(
            update.Id.ToString(),
            update.Note,
            update.CreatedAtUtc.ToString("yyyy-MM-dd HH:mm"),
            update.CreatedByName);

        return Result.Success(result);
    }
}
