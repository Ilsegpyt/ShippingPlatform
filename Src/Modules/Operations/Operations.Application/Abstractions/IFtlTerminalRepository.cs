using Operations.Domain.Entities;

namespace Operations.Application.Abstractions;

public interface IFtlTerminalRepository
{
    Task<IReadOnlyList<FtlTerminal>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default);
    Task<LookupPagedResult<FtlTerminal>> GetPagedAsync(string? search, string? status, int page, int pageSize, CancellationToken ct = default);
    Task<FtlTerminal?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<FtlTerminal?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<bool> NameExistsAsync(string name, Guid? excludeId = null, CancellationToken ct = default);
    Task AddAsync(FtlTerminal terminal, CancellationToken ct = default);
}

