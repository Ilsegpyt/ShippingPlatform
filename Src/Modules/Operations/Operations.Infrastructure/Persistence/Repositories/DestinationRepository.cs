using Microsoft.EntityFrameworkCore;
using Operations.Application.Abstractions;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Repositories;

public sealed class DestinationRepository(OperationsDbContext dbContext) : IDestinationRepository
{
    public async Task<IReadOnlyList<Destination>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Destinations.AsNoTracking();
        if (!includeInactive) query = query.Where(x => !x.IsDeleted && x.IsActive);
        return await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public Task<Destination?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Destinations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> DestinationNameExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var norm = name.Trim().ToLower();
        return dbContext.Destinations.AnyAsync(x => x.Name.ToLower() == norm && !x.IsDeleted && (excludeId == null || x.Id != excludeId), cancellationToken);
    }

    public async Task AddAsync(Destination destination, CancellationToken cancellationToken = default) =>
        await dbContext.Destinations.AddAsync(destination, cancellationToken);
}

