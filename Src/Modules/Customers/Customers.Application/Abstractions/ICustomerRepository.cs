using Customers.Domain;

namespace Customers.Application.Abstractions;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken ct);
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Customer>> ListAsync(CancellationToken ct); // بسيط دلوقتي، هيتطور لـ Pagination/Search لو احتجنا
                                                                   // في ICustomerRepository:
    Task<IReadOnlyList<Customer>> ListIgnoringDeletedFilterAsync(bool deletedOnly, CancellationToken ct);
    Task<Customer?> GetByOwnerUserIdAsync(Guid userId, CancellationToken ct);
}