using BuildingBlocks.Application;
using MediatR;
using Shipments.Application.Abstractions;
using Shipments.Application.Shipments.GetAllShipmentsQuery;

namespace Shipments.Application.Shipments.GetShipmentByIdQuery;

public sealed record GetShipmentByIdQuery(Guid Id)
    : IRequest<Result<ShipmentResponse>>;

public sealed class GetShipmentByIdQueryHandler(
    IShipmentRepository repository)
    : IRequestHandler<GetShipmentByIdQuery, Result<ShipmentResponse>>
{
    public async Task<Result<ShipmentResponse>> Handle(
        GetShipmentByIdQuery query,
        CancellationToken ct)
    {
        var shipment = await repository.GetByIdAsync(
            query.Id,
            ct);

        if (shipment is null)
        {
            return Result.Failure<ShipmentResponse>(
                "Shipment not found.");
        }

        var response = new ShipmentResponse(
            shipment.Id,
            shipment.ShipmentRef,
            shipment.CustomerId,
            shipment.ScheduleId,
            shipment.Mode,
            shipment.Carrier,
            shipment.ContainerType,
            shipment.Quantity,
            shipment.Rate,
            shipment.Total,
            shipment.Status,
            shipment.MBL,
            shipment.HBL,
            shipment.MAWB,
            shipment.BookingConfirmationNumber,
            shipment.CreatedAtUtc);

        return Result.Success(response);
    }
}