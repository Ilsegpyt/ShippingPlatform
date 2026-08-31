namespace Identity.Domain.Repositories;

public interface IAccountManagerAssignmentRepository
{
    Task<AccountManagerAssignment?> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken ct = default);

    void Add(AccountManagerAssignment assignment);

    void Update(AccountManagerAssignment assignment);

    void Delete(AccountManagerAssignment assignment);
}