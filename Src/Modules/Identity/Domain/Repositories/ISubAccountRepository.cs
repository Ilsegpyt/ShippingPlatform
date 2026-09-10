
using Identity.Domain.Entities;

namespace Identity.Domain.Repositories;

public interface ISubAccountRepository
{
    Task<SubAccount?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<SubAccount?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<SubAccount>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
    void Add(SubAccount subAccount);
    void Update(SubAccount subAccount);
    void Delete(SubAccount subAccount);
}

