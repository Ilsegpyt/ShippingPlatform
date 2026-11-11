using System.Text.Json;
using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using MediatR;
using Operations.Application.Abstractions;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.Ftl.UpdateFtlStatus;

public sealed class UpdateFtlStatusCommandHandler(
    IFtlOperationRepository operationRepository,
    IOperationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<UpdateFtlStatusCommand, Result<UpdateFtlStatusResult>>
{
    public async Task<Result<UpdateFtlStatusResult>> Handle(
        UpdateFtlStatusCommand command,
        CancellationToken ct)
    {
        var operation = await operationRepository.GetByRefAsync(command.Ref, true, ct);
        if (operation is null)
            return Result.Failure<UpdateFtlStatusResult>($"Operation '{command.Ref}' not found.");

        if (command.Version.HasValue && command.Version.Value != operation.Version)
        {
            return Result.Failure<UpdateFtlStatusResult>(
                "Operation was modified by another user (409 Conflict).");
        }

        if (!Enum.TryParse<OperationStatus>(command.Status, true, out var targetStatus))
            return Result.Failure<UpdateFtlStatusResult>($"Invalid status '{command.Status}'.");

        var userId = currentUser.UserId != Guid.Empty ? currentUser.UserId.ToString() : "u-1";
        var userName = await currentUser.GetUserNameAsync(ct);
        var now = DateTime.UtcNow;

        operation.UpdateStatus(targetStatus, userId, userName, now);

        // Also update status inside trips fields JSON so it remains synchronized
        foreach (var trip in operation.Trips)
        {
            if (!string.IsNullOrWhiteSpace(trip.FieldsJson))
            {
                try
                {
                    var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(trip.FieldsJson);
                    if (dict is not null)
                    {
                        dict["status"] = targetStatus.ToString();
                        trip.Update(
                            trip.Ordinal,
                            trip.ExternalId,
                            trip.FtlShipmentId,
                            trip.FtlTruckId,
                            trip.LoadStatus,
                            trip.LoadedPackages,
                            trip.LoadedWeight,
                            trip.LoadedCbm,
                            JsonSerializer.Serialize(dict));
                    }
                }
                catch { /* Ignore deserialization failure */ }
            }
        }

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new UpdateFtlStatusResult(operation.Version));
    }
}
