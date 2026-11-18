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

        // Try to match truck from trip or by ordinal from LayoutJson
        var trip = operation.Trips.FirstOrDefault(t => 
            (!string.IsNullOrWhiteSpace(command.TripId) && (t.ExternalId == command.TripId || t.Id.ToString() == command.TripId)));

        short truckOrd = (short)(trip is not null ? trip.Ordinal + 1 : 1);
        if (!string.IsNullOrWhiteSpace(operation.LayoutJson))
        {
            try
            {
                using var doc = System.Text.Json.JsonDocument.Parse(operation.LayoutJson);
                if (doc.RootElement.TryGetProperty("trkOf", out var trkOfEl) && trkOfEl.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    var trkOfList = System.Text.Json.JsonSerializer.Deserialize<List<int>>(trkOfEl.GetRawText());
                    if (trkOfList is not null && trip is not null && trip.Ordinal < trkOfList.Count)
                    {
                        truckOrd = (short)trkOfList[trip.Ordinal];
                    }
                }
            }
            catch { }
        }

        var truck = trip?.Truck 
            ?? (trip?.FtlTruckId.HasValue == true ? operation.Trucks.FirstOrDefault(t => t.Id == trip.FtlTruckId.Value) : null)
            ?? operation.Trucks.FirstOrDefault(t => t.Ordinal == truckOrd);

        if (truck is null)
        {
            truck = new Domain.Entities.Ftl.FtlTruck(operation.Id, truckOrd);
            operation.Trucks.Add(truck);
            await operationRepository.AddTruckAsync(truck, ct);
        }

        var update = truck.AddUpdate(command.TripId, command.Note, userId, userName, now);
        await operationRepository.AddTruckUpdateAsync(update, ct);

        if (trip is not null)
        {
            trip.Update(
                trip.Ordinal,
                trip.ExternalId,
                trip.FtlShipmentId,
                truck.Id,
                trip.LoadStatus,
                trip.LoadedPackages,
                trip.LoadedWeight,
                trip.LoadedCbm);
        }

        await unitOfWork.SaveChangesAsync(ct);

        var result = new AddFtlTruckUpdateResult(
            update.Id.ToString(),
            update.Note,
            update.CreatedAtUtc.ToString("HH:mm"),
            update.CreatedByName);

        return Result.Success(result);
    }
}
