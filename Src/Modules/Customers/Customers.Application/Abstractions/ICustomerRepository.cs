using Customers.Domain.Entities;

namespace Customers.Application.Abstractions;

public interface ICustomerRepository
{
    void Add(Customer customer);
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Customer>> ListAsync(int skip, int take, CancellationToken ct);
    Task<int> CountAsync(CancellationToken ct);
    Task<IReadOnlyList<Customer>> ListIgnoringDeletedFilterAsync(bool deletedOnly, int skip, int take, CancellationToken ct);
    Task<int> CountIgnoringDeletedFilterAsync(bool deletedOnly, CancellationToken ct);
    Task<Customer?> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<Customer?> GetOwnerByCustomerIdAsync(Guid customerId, CancellationToken ct);


}