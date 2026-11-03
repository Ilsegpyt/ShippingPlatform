using Microsoft.EntityFrameworkCore;
using Operations.Application.Abstractions;
using Operations.Domain.Entities;

namespace Operations.Infrastructure.Persistence.Repositories;

public sealed class VehicleRepository(OperationsDbContext dbContext) : IVehicleRepository
{
    public async Task<IReadOnlyList<Vehicle>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Vehicles.AsNoTracking();
        if (!includeInactive) query = query.Where(x => !x.IsDeleted && x.IsActive);
        return await query.OrderBy(x => x.CarNumber).ToListAsync(cancellationToken);
    }

    public Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Vehicles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<Vehicle?> GetByCarNumberAsync(string carNumber, CancellationToken cancellationToken = default)
    {
        var normalized = carNumber.Trim().ToLower();
        return dbContext.Vehicles.FirstOrDefaultAsync(
            x => x.CarNumber.ToLower() == normalized && !x.IsDeleted && x.IsActive,
            cancellationToken);
    }

    public Task<bool> VehicleCarNumberExistsAsync(string carNumber, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var norm = carNumber.Trim().ToLower();
        return dbContext.Vehicles.AnyAsync(x => x.CarNumber.ToLower() == norm && !x.IsDeleted && (excludeId == null || x.Id != excludeId), cancellationToken);
    }

    public async Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default) =>
        await dbContext.Vehicles.AddAsync(vehicle, cancellationToken);
}
