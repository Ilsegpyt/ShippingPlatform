using Operations.Domain.Entities;

namespace Operations.Application.Abstractions;

public interface IVehicleRepository
{
    Task<IReadOnlyList<Vehicle>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Vehicle?> GetByCarNumberAsync(string carNumber, CancellationToken cancellationToken = default);
    Task<bool> VehicleCarNumberExistsAsync(string carNumber, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default);
}
