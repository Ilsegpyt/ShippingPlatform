using Customers.Domain.Entities;

namespace Customers.Application.Abstractions;

public interface ICustomerRepository
{
    void Add(Customer customer);
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Customer>> ListAsync(CancellationToken ct); 
    Task<IReadOnlyList<Customer>> ListIgnoringDeletedFilterAsync(bool deletedOnly, CancellationToken ct);
    Task<Customer?> GetByUserIdAsync(Guid userId, CancellationToken ct);
}