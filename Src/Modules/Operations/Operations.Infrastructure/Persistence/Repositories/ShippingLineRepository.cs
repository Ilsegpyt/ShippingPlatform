using Microsoft.EntityFrameworkCore;
using Operations.Application.Abstractions;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Repositories;

public sealed class ShippingLineRepository(OperationsDbContext dbContext) : IShippingLineRepository
{
    public async Task<IReadOnlyList<ShippingLine>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = dbContext.ShippingLines.AsNoTracking();
        if (!includeInactive) query = query.Where(x => !x.IsDeleted && x.IsActive);
        return await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public async Task<LookupPagedResult<ShippingLine>> GetPagedAsync(
        string? search,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = dbContext.ShippingLines.AsNoTracking().Where(x => !x.IsDeleted);

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
                                     (x.Abbreviation != null && x.Abbreviation.ToLower().Contains(term)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new LookupPagedResult<ShippingLine>(items, totalCount, activeCount, inactiveCount);
    }

    public Task<ShippingLine?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.ShippingLines.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> ShippingLineNameExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var norm = name.Trim().ToLower();
        return dbContext.ShippingLines.AnyAsync(x => x.Name.ToLower() == norm && !x.IsDeleted && (excludeId == null || x.Id != excludeId), cancellationToken);
    }

    public async Task AddAsync(ShippingLine shippingLine, CancellationToken cancellationToken = default) =>
        await dbContext.ShippingLines.AddAsync(shippingLine, cancellationToken);
}
