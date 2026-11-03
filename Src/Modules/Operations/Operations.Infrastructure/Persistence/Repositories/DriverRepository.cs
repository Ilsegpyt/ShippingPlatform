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

