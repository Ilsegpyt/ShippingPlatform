using Identity.Domain;
using Identity.Domain.Repositories;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Identity.Infrastructure.Repositories;

public sealed class RoleRepository : IRoleRepository
{
    private readonly IdentityDbContext _db;

    public RoleRepository(IdentityDbContext db) => _db = db;

    public async Task<Role?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.BusinessRoles.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<Role?> GetByNameAsync(string name, CancellationToken ct = default) =>
        await _db.BusinessRoles.FirstOrDefaultAsync(x => x.Name == name, ct);

    public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken ct = default) =>
        await _db.BusinessRoles.ToListAsync(ct);

    public void Add(Role role) => _db.BusinessRoles.Add(role);

    public void Update(Role role) => _db.BusinessRoles.Update(role);
}

