using Identity.Domain;
using Identity.Domain.Repositories;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories;

public sealed class SubAccountRepository : ISubAccountRepository
{
    private readonly IdentityDbContext _db;

    public SubAccountRepository(IdentityDbContext db) => _db = db;

    public async Task<SubAccount?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.SubAccounts.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<SubAccount?> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await _db.SubAccounts.FirstOrDefaultAsync(x => x.UserId == userId, ct);

    public async Task<IReadOnlyList<SubAccount>> GetByOrganizationIdAsync(Guid organizationId, CancellationToken ct = default) =>
        await _db.SubAccounts.Where(x => x.OrganizationId == organizationId).ToListAsync(ct);
    public void Add(SubAccount subAccount) => _db.SubAccounts.Add(subAccount);

    public void Update(SubAccount subAccount) => _db.SubAccounts.Update(subAccount);

    public void Delete(SubAccount subAccount) => _db.SubAccounts.Remove(subAccount);
    
}