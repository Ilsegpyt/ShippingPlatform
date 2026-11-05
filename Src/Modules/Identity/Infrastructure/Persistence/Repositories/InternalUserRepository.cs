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

    public async Task<IReadOnlyList<InternalUser>> GetAllAsync(int skip, int take, string? search = null, CancellationToken ct = default)
    {
        var query = _db.InternalUsers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(x =>
                EF.Functions.Like(x.Name, $"%{s}%") ||
                EF.Functions.Like(x.Email, $"%{s}%") ||
                EF.Functions.Like(x.Phone, $"%{s}%"));
        }

        return await query
            .OrderBy(x => x.Name)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? search = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(search))
            return await _db.InternalUsers.CountAsync(ct);

        var s = search.Trim();
        return await _db.InternalUsers
            .Where(x =>
                EF.Functions.Like(x.Name, $"%{s}%") ||
                EF.Functions.Like(x.Email, $"%{s}%") ||
                EF.Functions.Like(x.Phone, $"%{s}%"))
            .CountAsync(ct);
    }

    public async Task<IReadOnlyList<InternalUser>> GetByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken ct = default)
    {
        if (ids.Count == 0)
            return [];

        return await _db.InternalUsers
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<InternalUser>> GetByRoleIdAsync(
        Guid roleId,
        CancellationToken ct = default)
    {
        return await _db.InternalUsers
            .AsNoTracking()
            .Where(x => x.RoleId == roleId)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
    }
}
