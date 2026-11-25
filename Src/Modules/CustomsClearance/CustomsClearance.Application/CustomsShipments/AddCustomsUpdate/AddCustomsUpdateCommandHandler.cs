using BuildingBlocks.Application;
using BuildingBlocks.Application.Abstractions;
using CustomsClearance.Application.Abstractions;
using CustomsClearance.Application.Abstractions.Repositories;
using CustomsClearance.Domain.Entities;
using CustomsClearance.Domain.Enums;
using MediatR;

namespace CustomsClearance.Application.CustomsShipments.AddCustomsUpdate;

public sealed class AddCustomsUpdateCommandHandler(
    ICustomsShipmentRepository customsShipmentRepository,
    ICustomsClearanceUnitOfWork customsClearanceUnitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<AddCustomsUpdateCommand, Result>
{
    public async Task<Result> Handle(
        AddCustomsUpdateCommand cmd,
        CancellationToken ct)
    {
        var shipment = await customsShipmentRepository.GetForUpdateAsync(cmd.ShipmentId, ct);

        if (shipment is null)
            return Result.Failure("Customs shipment was not found.");

        if (shipment.Status == CustomsShipmentStatus.Closed)
            return Result.Failure("Closed customs shipments are read-only.");

        var updateText = cmd.UpdateText.Trim();

        // Append to the history archive and refresh the grid snapshot.
        customsShipmentRepository.AddUpdateLog(new CustomsUpdateLog
        {
            ShipmentId = shipment.Id,
            UpdateText = updateText,
            UpdatedAtUtc = DateTime.UtcNow,
            UpdatedBy = await currentUser.GetUserNameAsync(ct)
        });

        shipment.LatestUpdate = updateText;

        await customsClearanceUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
