using Customers.Domain.Entities;

namespace Customers.Application.Abstractions;

public interface ICustomerRepository
{
    void Add(Customer customer);
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Customer>> ListAsync(int skip, int take, string? search = null, CancellationToken ct = default);
    Task<IReadOnlyList<Customer>> ListAsync(int skip, int take, CancellationToken ct) => ListAsync(skip, take, null, ct);
    Task<int> CountAsync(string? search = null, CancellationToken ct = default);
    Task<int> CountAsync(CancellationToken ct) => CountAsync(null, ct);
    Task<IReadOnlyList<Customer>> ListIgnoringDeletedFilterAsync(bool deletedOnly, int skip, int take, string? search = null, CancellationToken ct = default);
    Task<IReadOnlyList<Customer>> ListIgnoringDeletedFilterAsync(bool deletedOnly, int skip, int take, CancellationToken ct) => ListIgnoringDeletedFilterAsync(deletedOnly, skip, take, null, ct);
    Task<int> CountIgnoringDeletedFilterAsync(bool deletedOnly, string? search = null, CancellationToken ct = default);
    Task<int> CountIgnoringDeletedFilterAsync(bool deletedOnly, CancellationToken ct) => CountIgnoringDeletedFilterAsync(deletedOnly, null, ct);
    Task<Customer?> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<Customer?> GetOwnerByCustomerIdAsync(Guid customerId, CancellationToken ct);
    Task<IReadOnlyList<Customer>> ListByIdsAsync(IReadOnlyCollection<Guid> customerIds, int skip, int take, string? search = null, CancellationToken ct = default);
    Task<IReadOnlyList<Customer>> ListByIdsAsync(IReadOnlyCollection<Guid> customerIds, int skip, int take, CancellationToken ct) => ListByIdsAsync(customerIds, skip, take, null, ct);
    Task<int> CountByIdsAsync(IReadOnlyCollection<Guid> customerIds, string? search = null, CancellationToken ct = default);
    Task<int> CountByIdsAsync(IReadOnlyCollection<Guid> customerIds, CancellationToken ct) => CountByIdsAsync(customerIds, null, ct);
}
