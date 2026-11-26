using Microsoft.EntityFrameworkCore;
using Operations.Application.Abstractions;
using Operations.Domain.Entities.Fcl;

namespace Operations.Infrastructure.Persistence.Repositories;

public sealed class PortTerminalRepository(OperationsDbContext dbContext) : IPortTerminalRepository
{
    public async Task<IReadOnlyList<PortTerminal>> GetAllAsync(
        Guid? portId = null,
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.PortTerminals
            .AsNoTracking()
            .Include(x => x.Port)
            .AsQueryable();

        if (!includeInactive)
            query = query.Where(x => !x.IsDeleted && x.IsActive);
        else
            query = query.IgnoreQueryFilters();

        if (portId.HasValue)
            query = query.Where(x => x.PortId == portId.Value);

        return await query
            .OrderBy(x => x.Port.Name)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<LookupPagedResult<PortTerminal>> GetPagedAsync(
        string? search,
        string? status,
        Guid? portId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = dbContext.PortTerminals
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(x => x.Port)
            .AsQueryable();

        if (portId.HasValue)
            baseQuery = baseQuery.Where(x => x.PortId == portId.Value);

        var activeCount = await baseQuery.CountAsync(x => x.IsActive && !x.IsDeleted, cancellationToken);
        var inactiveCount = await baseQuery.CountAsync(x => !x.IsActive || x.IsDeleted, cancellationToken);

        var query = baseQuery;

        if (string.Equals(status, "active", StringComparison.OrdinalIgnoreCase))
            query = query.Where(x => x.IsActive && !x.IsDeleted);
        else if (string.Equals(status, "inactive", StringComparison.OrdinalIgnoreCase))
            query = query.Where(x => !x.IsActive || x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(x =>
                x.Name.ToLower().Contains(term) ||
                (x.Code != null && x.Code.ToLower().Contains(term)) ||
                (x.ArabicName != null && x.ArabicName.ToLower().Contains(term)) ||
                (x.Port != null && (x.Port.Name.ToLower().Contains(term) || (x.Port.Code != null && x.Port.Code.ToLower().Contains(term)))));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.Port.Name)
            .ThenBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new LookupPagedResult<PortTerminal>(items, totalCount, activeCount, inactiveCount);
    }

    public Task<PortTerminal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.PortTerminals
            .IgnoreQueryFilters()
            .Include(x => x.Port)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> NameExistsAsync(Guid portId, string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var norm = name.Trim().ToLower();
        return dbContext.PortTerminals.AnyAsync(
            x => x.PortId == portId && x.Name.ToLower() == norm && !x.IsDeleted && (excludeId == null || x.Id != excludeId),
            cancellationToken);
    }

    public Task<bool> CodeExistsAsync(Guid portId, string code, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var norm = code.Trim().ToUpper();
        return dbContext.PortTerminals.AnyAsync(
            x => x.PortId == portId && x.Code != null && x.Code.ToUpper() == norm && !x.IsDeleted && (excludeId == null || x.Id != excludeId),
            cancellationToken);
    }

    public async Task AddAsync(PortTerminal terminal, CancellationToken cancellationToken = default) =>
        await dbContext.PortTerminals.AddAsync(terminal, cancellationToken);
}
