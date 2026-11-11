using Microsoft.EntityFrameworkCore;
using Operations.Application.Abstractions;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Repositories;

public sealed class FtlTransporterRepository(OperationsDbContext dbContext) : IFtlTransporterRepository
{
    public async Task<IReadOnlyList<FtlTransporter>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default)
    {
        var query = dbContext.FtlTransporters.AsNoTracking();
        if (!includeInactive) query = query.Where(x => !x.IsDeleted && x.IsActive);
        return await query.OrderBy(x => x.Name).ToListAsync(ct);
    }

    public async Task<LookupPagedResult<FtlTransporter>> GetPagedAsync(
        string? search,
        string? status,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var baseQuery = dbContext.FtlTransporters.AsNoTracking().Where(x => !x.IsDeleted);

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
            query = query.Where(x => x.Name.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new LookupPagedResult<FtlTransporter>(items, totalCount, activeCount, inactiveCount);
    }

    public Task<FtlTransporter?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        dbContext.FtlTransporters.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<FtlTransporter?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        var trimmed = name.Trim().ToLower();
        return dbContext.FtlTransporters.FirstOrDefaultAsync(x => x.Name.ToLower() == trimmed, ct);
    }

    public Task<bool> NameExistsAsync(string name, Guid? excludeId = null, CancellationToken ct = default)
    {
        var trimmed = name.Trim().ToLower();
        var query = dbContext.FtlTransporters.Where(x => x.Name.ToLower() == trimmed);
        if (excludeId.HasValue) query = query.Where(x => x.Id != excludeId.Value);
        return query.AnyAsync(ct);
    }

    public async Task AddAsync(FtlTransporter transporter, CancellationToken ct = default) =>
        await dbContext.FtlTransporters.AddAsync(transporter, ct);
}

