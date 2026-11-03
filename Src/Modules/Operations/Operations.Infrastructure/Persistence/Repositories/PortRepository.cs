using Microsoft.EntityFrameworkCore;
using Operations.Application.Abstractions;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Repositories;

public sealed class PortRepository(OperationsDbContext dbContext) : IPortRepository
{
    public async Task<IReadOnlyList<Port>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Ports.AsNoTracking();
        if (!includeInactive) query = query.Where(x => !x.IsDeleted && x.IsActive);
        return await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public Task<Port?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Ports.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> PortCodeExistsAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var norm = code.Trim().ToUpper();
        return dbContext.Ports.AnyAsync(x => x.Code != null && x.Code.ToUpper() == norm && !x.IsDeleted && (excludeId == null || x.Id != excludeId), cancellationToken);
    }

    public Task<bool> PortNameExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var norm = name.Trim().ToLower();
        return dbContext.Ports.AnyAsync(x => x.Name.ToLower() == norm && !x.IsDeleted && (excludeId == null || x.Id != excludeId), cancellationToken);
    }

    public async Task AddAsync(Port port, CancellationToken cancellationToken = default) =>
        await dbContext.Ports.AddAsync(port, cancellationToken);
}

