using Microsoft.EntityFrameworkCore;
using Shipments.Application.Abstractions;
using Shipments.Domain.Shipments;

namespace Shipments.Infrastructure.Persistence.Repositories;

public sealed class ShipmentRepository : IShipmentRepository
{
    private readonly ShipmentsDbContext _dbContext;

    public ShipmentRepository(ShipmentsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        Shipment shipment,
        CancellationToken ct = default)
    {
        await _dbContext.Shipments.AddAsync(shipment, ct);
    }

    public async Task<Shipment?> GetByIdAsync(
        Guid shipmentId,
        CancellationToken ct = default)
    {
        return await _dbContext.Shipments
            .FirstOrDefaultAsync(x => x.Id == shipmentId, ct);
    }

    public async Task<IReadOnlyList<Shipment>> GetByIdsAsync(
        IReadOnlyCollection<Guid> shipmentIds,
        CancellationToken ct = default)
    {
        return await _dbContext.Shipments
            .Where(x => shipmentIds.Contains(x.Id))
            .ToListAsync(ct);
    }
    public async Task<IReadOnlyList<Shipment>> GetAllAsync(int skip, int take, CancellationToken ct = default)
    {
        return await _dbContext.Shipments
            .AsNoTracking()
            .OrderBy(x => x.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(CancellationToken ct = default)
    {
        return await _dbContext.Shipments.CountAsync(ct);
    }
    public void RemoveRange(
        IReadOnlyCollection<Shipment> shipments)
    {
        _dbContext.Shipments.RemoveRange(shipments);
    }
    public async Task<IReadOnlyList<Shipment>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
    {
        return await _dbContext.Shipments
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(ct);
    }
}