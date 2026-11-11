using Operations.Domain.Entities;

namespace Operations.Application.Abstractions;

public interface IFtlTransporterRepository
{
    Task<IReadOnlyList<FtlTransporter>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default);
    Task<LookupPagedResult<FtlTransporter>> GetPagedAsync(string? search, string? status, int page, int pageSize, CancellationToken ct = default);
    Task<FtlTransporter?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<FtlTransporter?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<bool> NameExistsAsync(string name, Guid? excludeId = null, CancellationToken ct = default);
    Task AddAsync(FtlTransporter transporter, CancellationToken ct = default);
}

