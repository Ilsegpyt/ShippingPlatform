using Shipments.Contracts;
using Shipments.Application.Abstractions;

namespace Shipments.Application.Services;

public sealed class ShipmentQueryService(
    IShipmentRepository shipmentRepository)
    : IShipmentQueryService
{
    public async Task<IReadOnlyList<ShipmentSearchResult>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct)
    {
        var shipments = await shipmentRepository.GetByCustomerIdAsync(
       customerId,
       ct);

        return shipments
            .Select(shipment => new ShipmentSearchResult(
                shipment.Id,
                shipment.ShipmentRef,
                shipment.CustomerId,
                shipment.ScheduleId,
                shipment.Status.ToString()))
            .ToList();
    }

    public async Task<ShipmentSearchResult?> GetByIdAsync(Guid shipmentId, CancellationToken ct)
    {
        var shipment = await shipmentRepository.GetByIdAsync(shipmentId, ct);

        if (shipment is null)
            return null;

        return new ShipmentSearchResult(
            shipment.Id,
            shipment.ShipmentRef,
            shipment.CustomerId,
            shipment.ScheduleId,
            shipment.Status.ToString());
    }

}