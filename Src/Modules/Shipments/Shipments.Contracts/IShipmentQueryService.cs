namespace Shipments.Contracts;

// Get a specific shipment based on ID
public interface IShipmentQueryService
{
    Task<ShipmentSearchResult?> GetByIdAsync(Guid shipmentId, CancellationToken ct);
    Task<IReadOnlyList<ShipmentSearchResult>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct);
}

public sealed record ShipmentSearchResult(Guid Id, string ShipmentRef, Guid CustomerId, Guid ScheduleId, string Status);



