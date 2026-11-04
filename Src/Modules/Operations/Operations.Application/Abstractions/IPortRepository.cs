using Operations.Domain.Entities;

namespace Operations.Application.Abstractions;

public interface IPortRepository
{
    Task<IReadOnlyList<Port>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<LookupPagedResult<Port>> GetPagedAsync(string? search, string? status, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Port?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> PortCodeExistsAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> PortNameExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Port port, CancellationToken cancellationToken = default);
}
