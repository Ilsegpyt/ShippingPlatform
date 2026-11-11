using Operations.Domain.Entities;

namespace Operations.Application.Abstractions;

public interface IFtlDriverRepository
{
    Task<IReadOnlyList<FtlDriver>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default);
    Task<LookupPagedResult<FtlDriver>> GetPagedAsync(string? search, string? status, int page, int pageSize, CancellationToken ct = default);
    Task<FtlDriver?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<FtlDriver?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<bool> NameOrNumberExistsAsync(string name, string? driverNumber = null, Guid? excludeId = null, CancellationToken ct = default);
    Task AddAsync(FtlDriver driver, CancellationToken ct = default);
}

