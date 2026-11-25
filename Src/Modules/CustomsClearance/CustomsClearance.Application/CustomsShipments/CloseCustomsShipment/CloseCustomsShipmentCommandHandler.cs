using BuildingBlocks.Application;
using CustomsClearance.Application.Abstractions;
using CustomsClearance.Application.Abstractions.Repositories;
using CustomsClearance.Domain.Enums;
using MediatR;

namespace CustomsClearance.Application.CustomsShipments.CloseCustomsShipment;

public sealed class CloseCustomsShipmentCommandHandler(
    ICustomsShipmentRepository customsShipmentRepository,
    ICustomsClearanceUnitOfWork customsClearanceUnitOfWork)
    : IRequestHandler<CloseCustomsShipmentCommand, Result>
{
    public async Task<Result> Handle(
        CloseCustomsShipmentCommand cmd,
        CancellationToken ct)
    {
        var shipment = await customsShipmentRepository.GetForUpdateAsync(cmd.Id, ct);

        if (shipment is null)
            return Result.Failure("Customs shipment was not found.");

        if (shipment.Status == CustomsShipmentStatus.Closed)
            return Result.Failure("Customs shipment is already closed.");

        shipment.Status = CustomsShipmentStatus.Closed;
        shipment.ClosedAtUtc = DateTime.UtcNow;

        await customsClearanceUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
