using BuildingBlocks.Application;
using MediatR;
using Shipments.Application.Abstractions;

namespace Shipments.Application.Shipments.UpdateShipment;

public sealed class UpdateShipmentCommandHandler(
    IShipmentRepository shipmentRepository,
    IShipmentsUnitOfWork unitOfWork)
    : IRequestHandler<
        UpdateShipmentCommand,
        Result<UpdateShipmentResponse>>
{
    public async Task<Result<UpdateShipmentResponse>> Handle(
        UpdateShipmentCommand command,
        CancellationToken ct)
    {
        var shipment = await shipmentRepository.GetByIdAsync(
            command.ShipmentId,
            ct);

        if (shipment is null)
        {
            return Result.Failure<UpdateShipmentResponse>(
                "Shipment was not found.");
        }

        // Sea shipment cannot have MAWB
        if (shipment.Mode.Equals("Sea", StringComparison.OrdinalIgnoreCase)
            && !string.IsNullOrWhiteSpace(command.MAWB))
        {
            return Result.Failure<UpdateShipmentResponse>(
                "Sea shipment cannot have MAWB.");
        }

        // Air shipment cannot have MBL
        if (shipment.Mode.Equals("Air", StringComparison.OrdinalIgnoreCase)
            && !string.IsNullOrWhiteSpace(command.MBL))
        {
            return Result.Failure<UpdateShipmentResponse>(
                "Air shipment cannot have MBL.");
        }

        // MBL and MAWB cannot both have values
        if (!string.IsNullOrWhiteSpace(command.MBL)
            && !string.IsNullOrWhiteSpace(command.MAWB))
        {
            return Result.Failure<UpdateShipmentResponse>(
                "MBL and MAWB cannot both have values.");
        }

        shipment.Update(
            command.Status,
            command.MBL,
            command.HBL,
            command.MAWB,
            command.BookingConfirmationNumber);

        await unitOfWork.SaveChangesAsync(ct);

        return new UpdateShipmentResponse(
            shipment.Id,
            shipment.ShipmentRef);
    }
}