using Microsoft.EntityFrameworkCore;
using Operations.Application.Abstractions;
using Operations.Domain.Entities;

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
        var trimmed = reference.Trim().ToUpperInvariant();
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

        return await query.FirstOrDefaultAsync(x => x.Ref.ToUpper() == trimmed, ct);
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

    public async Task<int> GetNextSequenceForYearAsync(int year, CancellationToken ct = default)
    {
        var maxSeq = await dbContext.FtlOperations
            .IgnoreQueryFilters()
            .Where(x => x.Year == year)
            .MaxAsync(x => (int?)x.Sequence, ct);

        return (maxSeq ?? 0) + 1;
    }

    public async Task<int> GetCurrentWorkspaceVersionAsync(CancellationToken ct = default)
    {
        var maxVersion = await dbContext.FtlOperations
            .MaxAsync(x => (int?)x.Version, ct);

        return maxVersion ?? 1;
    }

    public async Task AddAsync(FtlOperation operation, CancellationToken ct = default)
    {
        await dbContext.FtlOperations.AddAsync(operation, ct);
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

    public Task DeleteShipmentsAsync(IEnumerable<FtlShipment> shipments, CancellationToken ct = default)
    {
        dbContext.FtlShipments.RemoveRange(shipments);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsByRefAsync(string reference, CancellationToken ct = default)
    {
        var trimmed = reference.Trim().ToUpperInvariant();
        return dbContext.FtlOperations.AnyAsync(x => x.Ref.ToUpper() == trimmed, ct);
    }
}

