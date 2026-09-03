using Shipments.Domain.Shipments;

namespace Shipments.Application.Abstractions;

public interface IShipmentRepository
{
    Task AddAsync(
        Shipment shipment,
        CancellationToken ct = default);

    Task<Shipment?> GetByIdAsync(
        Guid shipmentId,
        CancellationToken ct = default);

    Task<IReadOnlyList<Shipment>> GetByIdsAsync(
        IReadOnlyCollection<Guid> shipmentIds,
        CancellationToken ct = default);

    void RemoveRange(
    IReadOnlyCollection<Shipment> shipments);
}