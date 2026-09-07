using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories;

public sealed class InternalUserRepository : IInternalUserRepository
{
    private readonly IdentityDbContext _db;

    public InternalUserRepository(IdentityDbContext db) => _db = db;

    public async Task<InternalUser?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.InternalUsers.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<InternalUser?> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await _db.InternalUsers.FirstOrDefaultAsync(x => x.UserId == userId, ct);

    public void Add(InternalUser internalUser) => 
        _db.InternalUsers.Add(internalUser);

    public void Update(InternalUser internalUser) => 
        _db.InternalUsers.Update(internalUser);

    public void Delete(InternalUser internalUser) =>
    _db.InternalUsers.Remove(internalUser);
    public async Task<IReadOnlyList<InternalUser>> GetAllAsync(int skip , int take, CancellationToken ct = default)
    {
        return await _db.InternalUsers
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }
    public async Task<int> CountAsync(CancellationToken ct = default)
    {
        return await _db.InternalUsers.CountAsync(ct);

    }
}
