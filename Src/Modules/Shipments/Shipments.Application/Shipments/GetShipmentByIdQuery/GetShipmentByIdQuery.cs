using BuildingBlocks.Application;
using Customers.Contracts;
using MediatR;
using Shipments.Application.Abstractions;
using Shipments.Application.Shipments.GetAllShipmentsQuery;

namespace Shipments.Application.Shipments.GetShipmentByIdQuery;

public sealed record GetShipmentByIdQuery(Guid Id)
    : IRequest<Result<ShipmentResponse>>;

public sealed class GetShipmentByIdQueryHandler(
    IShipmentRepository repository,
    ICustomerQueries customerQueries)
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

        var customers =
            await customerQueries.GetByIdsAsync(
                [shipment.CustomerId],
                ct);

        var customerName =
            customers
                .FirstOrDefault()
                ?.CompanyName
            ?? "Unknown Customer";

        var response = new ShipmentResponse(
            shipment.Id,
            shipment.ShipmentRef,
            shipment.CustomerId,
            customerName,
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