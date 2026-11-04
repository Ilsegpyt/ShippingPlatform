using Operations.Domain.Entities;

namespace Operations.Application.Abstractions;

public interface IDriverRepository
{
    Task<IReadOnlyList<Driver>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<LookupPagedResult<Driver>> GetPagedAsync(string? search, string? status, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Driver?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> DriverNumberExistsAsync(string driverNumber, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Driver driver, CancellationToken cancellationToken = default);
}
