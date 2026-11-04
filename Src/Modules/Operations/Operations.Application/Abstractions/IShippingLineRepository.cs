using Operations.Domain.Entities;

namespace Operations.Application.Abstractions;

public interface IShippingLineRepository
{
    Task<IReadOnlyList<ShippingLine>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<LookupPagedResult<ShippingLine>> GetPagedAsync(string? search, string? status, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<ShippingLine?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ShippingLineNameExistsAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(ShippingLine shippingLine, CancellationToken cancellationToken = default);
}
