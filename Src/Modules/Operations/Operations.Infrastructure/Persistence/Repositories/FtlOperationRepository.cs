using Microsoft.EntityFrameworkCore;
using Operations.Application.Abstractions;
using Operations.Domain.Entities.Ftl;

namespace Operations.Infrastructure.Persistence.Repositories;

public sealed class FtlOperationRepository(OperationsDbContext dbContext) : IFtlOperationRepository
{
    public async Task<IReadOnlyList<FtlOperation>> GetAllActiveWithDetailsAsync(CancellationToken ct = default)
    {
        return await dbContext.FtlOperations
            .Include(x => x.Shipments)
            .Include(x => x.Trucks)
                .ThenInclude(t => t.Updates)
            .Include(x => x.Trips)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(ct);
    }

    public async Task<FtlOperation?> GetByRefAsync(string reference, bool includeDetails = true, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(reference)) return null;

        var clean = reference.Trim();
        if (clean.StartsWith("OPS-", StringComparison.OrdinalIgnoreCase))
        {
            clean = clean[4..];
        }

        var query = dbContext.FtlOperations.AsQueryable();

        if (includeDetails)
        {
            query = query
                .Include(x => x.Shipments)
                .Include(x => x.Trucks)
                    .ThenInclude(t => t.Updates)
                .Include(x => x.Trips)
                .Include(x => x.StatusAudits);
        }

        if (Guid.TryParse(clean, out var parsedGuid))
        {
            var byId = await query.FirstOrDefaultAsync(x => x.Id == parsedGuid, ct);
            if (byId is not null) return byId;
        }

        var lowerPrefix = clean.ToLowerInvariant();
        var match = await query.FirstOrDefaultAsync(x => x.Id.ToString().StartsWith(lowerPrefix), ct);
        if (match is not null) return match;

        var upperPrefix = clean.ToUpperInvariant();
        return await query.FirstOrDefaultAsync(x => x.Id.ToString().StartsWith(upperPrefix), ct);
    }

    public async Task<FtlOperation?> GetByIdAsync(Guid id, bool includeDetails = true, CancellationToken ct = default)
    {
        var query = dbContext.FtlOperations.AsQueryable();

        if (includeDetails)
        {
            query = query
                .Include(x => x.Shipments)
                .Include(x => x.Trucks)
                    .ThenInclude(t => t.Updates)
                .Include(x => x.Trips)
                .Include(x => x.StatusAudits);
        }

        return await query.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task AddAsync(FtlOperation operation, CancellationToken ct = default)
    {
        await dbContext.FtlOperations.AddAsync(operation, ct);
    }

    public async Task AddShipmentAsync(FtlShipment shipment, CancellationToken ct = default)
    {
        await dbContext.FtlShipments.AddAsync(shipment, ct);
    }

    public async Task AddTruckAsync(FtlTruck truck, CancellationToken ct = default)
    {
        await dbContext.FtlTrucks.AddAsync(truck, ct);
    }

    public async Task AddTruckUpdateAsync(FtlTruckUpdate update, CancellationToken ct = default)
    {
        await dbContext.FtlTruckUpdates.AddAsync(update, ct);
    }

    public async Task AddTripAsync(FtlOperationTrip trip, CancellationToken ct = default)
    {
        await dbContext.FtlOperationTrips.AddAsync(trip, ct);
    }

    public Task DeleteShipmentsAsync(IEnumerable<FtlShipment> shipments, CancellationToken ct = default)
    {
        dbContext.FtlShipments.RemoveRange(shipments);
        return Task.CompletedTask;
    }

    public Task DeleteTripsAsync(IEnumerable<FtlOperationTrip> trips, CancellationToken ct = default)
    {
        dbContext.FtlOperationTrips.RemoveRange(trips);
        return Task.CompletedTask;
    }

    public Task DeleteTrucksAsync(IEnumerable<FtlTruck> trucks, CancellationToken ct = default)
    {
        dbContext.FtlTrucks.RemoveRange(trucks);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsByRefAsync(string reference, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(reference)) return false;
        var clean = reference.Trim();
        if (clean.StartsWith("OPS-", StringComparison.OrdinalIgnoreCase)) clean = clean[4..];
        if (Guid.TryParse(clean, out var parsedGuid))
        {
            return await dbContext.FtlOperations.AnyAsync(x => x.Id == parsedGuid, ct);
        }
        var prefix = clean.ToLowerInvariant();
        return await dbContext.FtlOperations.AnyAsync(x => x.Id.ToString().StartsWith(prefix), ct);
    }

    public async Task<bool> ShipmentNumberExistsAsync(string shipNo, Guid? excludeOperationId = null, CancellationToken ct = default)
    {
        var norm = Operations.Domain.Common.IdentifierNormalizer.Normalize(shipNo);
        if (string.IsNullOrWhiteSpace(norm)) return false;
        return await dbContext.FtlShipments
            .AnyAsync(s => !s.Operation.IsDeleted && s.ShipmentNumber == norm && (!excludeOperationId.HasValue || s.FtlOperationId != excludeOperationId.Value), ct);
    }

    public async Task<bool> HawbExistsAsync(string hawb, Guid? excludeOperationId = null, CancellationToken ct = default)
    {
        var norm = Operations.Domain.Common.IdentifierNormalizer.Normalize(hawb);
        if (string.IsNullOrWhiteSpace(norm)) return false;
        return await dbContext.FtlShipments
            .AnyAsync(s => !s.Operation.IsDeleted && s.HAWB == norm && (!excludeOperationId.HasValue || s.FtlOperationId != excludeOperationId.Value), ct);
    }

    public async Task<bool> MawbHawbPairExistsAsync(string mawb, string hawb, Guid? excludeOperationId = null, CancellationToken ct = default)
    {
        var mNorm = Operations.Domain.Common.IdentifierNormalizer.Normalize(mawb);
        var hNorm = Operations.Domain.Common.IdentifierNormalizer.Normalize(hawb);
        if (string.IsNullOrWhiteSpace(mNorm) || string.IsNullOrWhiteSpace(hNorm)) return false;
        return await dbContext.FtlShipments
            .AnyAsync(s => !s.Operation.IsDeleted && s.MAWB == mNorm && s.HAWB == hNorm && (!excludeOperationId.HasValue || s.FtlOperationId != excludeOperationId.Value), ct);
    }

    public async Task<bool> CertShipmentPairExistsAsync(string cert, string shipNo, Guid? excludeOperationId = null, CancellationToken ct = default)
    {
        var cNorm = Operations.Domain.Common.IdentifierNormalizer.Normalize(cert);
        var sNorm = Operations.Domain.Common.IdentifierNormalizer.Normalize(shipNo);
        if (string.IsNullOrWhiteSpace(cNorm) || string.IsNullOrWhiteSpace(sNorm)) return false;
        return await dbContext.FtlShipments
            .AnyAsync(s => !s.Operation.IsDeleted
                && s.CertificateNumber == cNorm
                && s.ShipmentNumber == sNorm
                && (!excludeOperationId.HasValue || s.FtlOperationId != excludeOperationId.Value), ct);
    }
}

