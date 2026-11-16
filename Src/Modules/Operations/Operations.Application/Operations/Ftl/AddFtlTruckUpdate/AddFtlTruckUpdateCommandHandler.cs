using BuildingBlocks.Application;
using BuildingBlocks.Application.Serialization;
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
        var trip = operation.Trips.FirstOrDefault(t => 
            (!string.IsNullOrWhiteSpace(command.TripId) && (t.ExternalId == command.TripId || t.Id.ToString() == command.TripId)));
        var truck = trip?.Truck 
            ?? (trip?.FtlTruckId.HasValue == true ? operation.Trucks.FirstOrDefault(t => t.Id == trip.FtlTruckId.Value) : null)
            ?? operation.Trucks.FirstOrDefault();

        if (truck is null)
        {
            // Create a default truck if none exists yet
            truck = new Domain.Entities.Ftl.FtlTruck(operation.Id, 1);
            operation.Trucks.Add(truck);
            await operationRepository.AddTruckAsync(truck, ct);
        }

        var update = truck.AddUpdate(command.TripId, command.Note, userId, userName, now);
        await operationRepository.AddTruckUpdateAsync(update, ct);

        if (trip is not null)
        {
            Dictionary<string, string> fields;
            if (!string.IsNullOrWhiteSpace(trip.FieldsJson))
            {
                try
                {
                    fields = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(trip.FieldsJson, SafeStringDictionaryConverter.DefaultOptions) ?? new();
                }
                catch
                {
                    fields = new();
                }
            }
            else
            {
                fields = new();
            }

            fields["updates"] = command.Note.Trim();
            fields["updDate"] = $"{now:yyyy-MM-dd} {now:HH:mm}";
            var newFieldsJson = System.Text.Json.JsonSerializer.Serialize(fields);

            trip.Update(
                trip.Ordinal,
                trip.ExternalId,
                truck.Id,
                trip.LoadStatus,
                trip.LoadedPackages,
                trip.LoadedWeight,
                trip.LoadedCbm,
                newFieldsJson);
        }

        operation.IncrementVersion();

        await unitOfWork.SaveChangesAsync(ct);

        var result = new AddFtlTruckUpdateResult(
            update.Id.ToString(),
            update.Note,
            update.CreatedAtUtc.ToString("HH:mm"),
            update.CreatedByName,
            operation.Version);

        return Result.Success(result);
    }
}
