using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories;

public sealed class AccountManagerAssignmentRepository
    : IAccountManagerAssignmentRepository
{
    private readonly IdentityDbContext _db;

    public AccountManagerAssignmentRepository(IdentityDbContext db)
        => _db = db;
    public async Task<IReadOnlyList<AccountManagerAssignment>>
        ListByAccountManagerIdAsync(
         Guid accountManagerId,
         CancellationToken ct = default) =>
         await _db.AccountManagerAssignments
         .AsNoTracking()
         .Where(x => x.AccountManagerId == accountManagerId)
         .ToListAsync(ct);

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

    public async Task<IReadOnlyList<AccountManagerAssignment>> ListAllAsync(
    CancellationToken ct = default)
    {
        return await _db.AccountManagerAssignments
            .AsNoTracking()
            .ToListAsync(ct);
    }
}