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

