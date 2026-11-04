using Microsoft.EntityFrameworkCore;
using Operations.Application.Abstractions;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Repositories;

public sealed class TransporterRepository(OperationsDbContext dbContext) : ITransporterRepository
{
    public async Task<IReadOnlyList<Transporter>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Transporters.AsNoTracking();
        if (!includeInactive) query = query.Where(x => !x.IsDeleted && x.IsActive);
        return await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public async Task<LookupPagedResult<Transporter>> GetPagedAsync(
        string? search,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = dbContext.Transporters.AsNoTracking().Where(x => !x.IsDeleted);

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
            query = query.Where(x => x.Name.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new LookupPagedResult<Transporter>(items, totalCount, activeCount, inactiveCount);
    }

    public Task<Transporter?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Transporters.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> TransporterNameExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var norm = name.Trim().ToLower();
        return dbContext.Transporters.AnyAsync(x => x.Name.ToLower() == norm && !x.IsDeleted && (excludeId == null || x.Id != excludeId), cancellationToken);
    }

    public async Task AddAsync(Transporter transporter, CancellationToken cancellationToken = default) =>
        await dbContext.Transporters.AddAsync(transporter, cancellationToken);
}
