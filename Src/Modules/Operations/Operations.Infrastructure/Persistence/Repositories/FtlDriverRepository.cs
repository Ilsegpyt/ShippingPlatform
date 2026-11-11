using Microsoft.EntityFrameworkCore;
using Operations.Application.Abstractions;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Repositories;

public sealed class FtlDriverRepository(OperationsDbContext dbContext) : IFtlDriverRepository
{
    public async Task<IReadOnlyList<FtlDriver>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default)
    {
        var query = dbContext.FtlDrivers.AsNoTracking();
        if (!includeInactive) query = query.Where(x => !x.IsDeleted && x.IsActive);
        return await query.OrderBy(x => x.Name).ToListAsync(ct);
    }

    public async Task<LookupPagedResult<FtlDriver>> GetPagedAsync(
        string? search,
        string? status,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var baseQuery = dbContext.FtlDrivers.AsNoTracking().Where(x => !x.IsDeleted);

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
            query = query.Where(x => x.Name.ToLower().Contains(term)
                || (x.MobileNumber != null && x.MobileNumber.Contains(term))
                || (x.DriverNumber != null && x.DriverNumber.Contains(term)));
        }

        var totalCount = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new LookupPagedResult<FtlDriver>(items, totalCount, activeCount, inactiveCount);
    }

    public Task<FtlDriver?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        dbContext.FtlDrivers.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<FtlDriver?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        var trimmed = name.Trim().ToLower();
        return dbContext.FtlDrivers.FirstOrDefaultAsync(x => x.Name.ToLower() == trimmed, ct);
    }

    public Task<bool> NameOrNumberExistsAsync(string name, string? driverNumber = null, Guid? excludeId = null, CancellationToken ct = default)
    {
        var trimmedName = name.Trim().ToLower();
        var trimmedNum = driverNumber?.Trim().ToLower();

        var query = dbContext.FtlDrivers.Where(x =>
            x.Name.ToLower() == trimmedName ||
            (trimmedNum != null && x.DriverNumber != null && x.DriverNumber.ToLower() == trimmedNum));

        if (excludeId.HasValue) query = query.Where(x => x.Id != excludeId.Value);
        return query.AnyAsync(ct);
    }

    public async Task AddAsync(FtlDriver driver, CancellationToken ct = default) =>
        await dbContext.FtlDrivers.AddAsync(driver, ct);
}

