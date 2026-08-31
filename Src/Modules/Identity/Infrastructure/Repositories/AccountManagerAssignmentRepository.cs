using Identity.Domain;
using Identity.Domain.Repositories;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories;

public sealed class AccountManagerAssignmentRepository
    : IAccountManagerAssignmentRepository
{
    private readonly IdentityDbContext _db;

    public AccountManagerAssignmentRepository(IdentityDbContext db)
        => _db = db;

    public async Task<AccountManagerAssignment?> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken ct = default) =>
        await _db.AccountManagerAssignments
            .FirstOrDefaultAsync(x => x.CustomerId == customerId, ct);

    public void Add(AccountManagerAssignment assignment) =>
        _db.AccountManagerAssignments.Add(assignment);

    public void Update(AccountManagerAssignment assignment) =>
        _db.AccountManagerAssignments.Update(assignment);

    public void Delete(AccountManagerAssignment assignment) =>
        _db.AccountManagerAssignments.Remove(assignment);
}