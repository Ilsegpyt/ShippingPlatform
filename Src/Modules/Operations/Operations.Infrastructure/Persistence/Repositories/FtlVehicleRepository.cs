using Microsoft.EntityFrameworkCore;
using Operations.Application.Abstractions;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Repositories;

public sealed class FtlVehicleRepository(OperationsDbContext dbContext) : IFtlVehicleRepository
{
    public async Task<IReadOnlyList<FtlVehicle>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default)
    {
        var query = dbContext.FtlVehicles.Include(x => x.Transporter).AsNoTracking();
        if (!includeInactive) query = query.Where(x => !x.IsDeleted && x.IsActive);
        return await query.OrderBy(x => x.PlateNumber).ToListAsync(ct);
    }

    public async Task<LookupPagedResult<FtlVehicle>> GetPagedAsync(
        string? search,
        string? status,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var baseQuery = dbContext.FtlVehicles.AsNoTracking().Where(x => !x.IsDeleted);

        var activeCount = await baseQuery.CountAsync(x => x.IsActive, ct);
        var inactiveCount = await baseQuery.CountAsync(x => !x.IsActive, ct);

        var query = baseQuery;

        if (string.Equals(status, "active", StringComparison.OrdinalIgnoreCase))
            query = query.Where(x => x.IsActive);
        else if (string.Equals(status, "inactive", StringComparison.OrdinalIgnoreCase))
            query = query.Where(x => !x.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(x => x.PlateNumber.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync(ct);
        var items = await query.Include(x => x.Transporter).OrderBy(x => x.PlateNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new LookupPagedResult<FtlVehicle>(items, totalCount, activeCount, inactiveCount);
    }

    public Task<FtlVehicle?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        dbContext.FtlVehicles.Include(x => x.Transporter).FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<FtlVehicle?> GetByPlateNumberAsync(string plateNumber, CancellationToken ct = default)
    {
        var trimmed = plateNumber.Trim().ToLower();
        return dbContext.FtlVehicles.Include(x => x.Transporter).FirstOrDefaultAsync(x => x.PlateNumber.ToLower() == trimmed, ct);
    }

    public Task<bool> PlateNumberExistsAsync(string plateNumber, Guid? excludeId = null, CancellationToken ct = default)
    {
        var trimmed = plateNumber.Trim().ToLower();
        var query = dbContext.FtlVehicles.Where(x => x.PlateNumber.ToLower() == trimmed);
        if (excludeId.HasValue) query = query.Where(x => x.Id != excludeId.Value);
        return query.AnyAsync(ct);
    }

    public async Task AddAsync(FtlVehicle vehicle, CancellationToken ct = default) =>
        await dbContext.FtlVehicles.AddAsync(vehicle, ct);
}

