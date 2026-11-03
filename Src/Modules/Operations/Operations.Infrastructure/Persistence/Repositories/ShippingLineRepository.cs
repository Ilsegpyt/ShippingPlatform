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

