using Microsoft.EntityFrameworkCore;
using Operations.Application.Abstractions;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Repositories;

public sealed class DriverRepository(OperationsDbContext dbContext) : IDriverRepository
{
    public async Task<IReadOnlyList<Driver>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Drivers.AsNoTracking();
        if (!includeInactive) query = query.Where(x => !x.IsDeleted && x.IsActive);
        return await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public async Task<LookupPagedResult<Driver>> GetPagedAsync(
        string? search,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = dbContext.Drivers.AsNoTracking().Where(x => !x.IsDeleted);

        var activeCount = await baseQuery.CountAsync(x => x.IsActive, cancellationToken);
        var inactiveCount = await baseQuery.CountAsync(x => !x.IsActive, cancellationToken);

        var query = baseQuery;

        if (string.Equals(status, "active", StringComparison.OrdinalIgnoreCase))
            query = query.Where(x => x.IsActive);
        else if (string.Equals(status, "inactive", StringComparison.OrdinalIgnoreCase))
            query = query.Where(x => !x.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(term) ||
                                     x.DriverNumber.ToLower().Contains(term) ||
                                     (x.MobileNumber != null && x.MobileNumber.ToLower().Contains(term)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new LookupPagedResult<Driver>(items, totalCount, activeCount, inactiveCount);
    }

    public Task<Driver?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Drivers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> DriverNumberExistsAsync(string driverNumber, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var norm = driverNumber.Trim().ToLower();
        return dbContext.Drivers.AnyAsync(x => x.DriverNumber.ToLower() == norm && !x.IsDeleted && (excludeId == null || x.Id != excludeId), cancellationToken);
    }

    public async Task AddAsync(Driver driver, CancellationToken cancellationToken = default) =>
        await dbContext.Drivers.AddAsync(driver, cancellationToken);
}
