using Operations.Domain.Entities.Fcl;

namespace Operations.Application.Abstractions;

public interface IPortTerminalRepository
{
    Task<IReadOnlyList<PortTerminal>> GetAllAsync(Guid? portId = null, bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<LookupPagedResult<PortTerminal>> GetPagedAsync(string? search, string? status, Guid? portId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<PortTerminal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(Guid portId, string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(Guid portId, string code, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(PortTerminal terminal, CancellationToken cancellationToken = default);
}
