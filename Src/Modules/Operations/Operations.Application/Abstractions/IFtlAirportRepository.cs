using Operations.Domain.Entities;

namespace Operations.Application.Abstractions;

public interface IFtlAirportRepository
{
    Task<IReadOnlyList<FtlAirport>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default);
    Task<LookupPagedResult<FtlAirport>> GetPagedAsync(string? search, string? status, int page, int pageSize, CancellationToken ct = default);
    Task<FtlAirport?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<FtlAirport?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<bool> NameExistsAsync(string name, Guid? excludeId = null, CancellationToken ct = default);
    Task AddAsync(FtlAirport airport, CancellationToken ct = default);
}

