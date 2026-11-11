using Operations.Domain.Entities;

namespace Operations.Application.Abstractions;

public interface IFtlVehicleRepository
{
    Task<IReadOnlyList<FtlVehicle>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default);
    Task<LookupPagedResult<FtlVehicle>> GetPagedAsync(string? search, string? status, int page, int pageSize, CancellationToken ct = default);
    Task<FtlVehicle?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<FtlVehicle?> GetByPlateNumberAsync(string plateNumber, CancellationToken ct = default);
    Task<bool> PlateNumberExistsAsync(string plateNumber, Guid? excludeId = null, CancellationToken ct = default);
    Task AddAsync(FtlVehicle vehicle, CancellationToken ct = default);
}

