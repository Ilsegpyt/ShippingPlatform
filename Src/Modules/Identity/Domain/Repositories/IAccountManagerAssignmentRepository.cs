using Identity.Domain.Entities;

namespace Identity.Domain.Repositories;

public interface IAccountManagerAssignmentRepository
{
    Task<AccountManagerAssignment?> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
    Task<IReadOnlyList<AccountManagerAssignment>> ListByAccountManagerIdAsync(
    Guid accountManagerId,
    CancellationToken ct = default);
    void Add(AccountManagerAssignment assignment);

    void Update(AccountManagerAssignment assignment);

    void Delete(AccountManagerAssignment assignment);

    Task<IReadOnlyList<AccountManagerAssignment>> ListAllAsync(
    CancellationToken ct = default);
}