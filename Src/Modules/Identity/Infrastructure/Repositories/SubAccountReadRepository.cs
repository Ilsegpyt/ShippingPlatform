using Microsoft.EntityFrameworkCore;
using Identity.Application.SubAccounts.GetSubAccounts;
using Identity.Domain;
using Identity.Infrastructure.Persistence;

namespace Identity.Infrastructure.Repositories;

public sealed class SubAccountReadRepository : ISubAccountReadRepository
{
    private readonly IdentityDbContext _db;

    public SubAccountReadRepository(IdentityDbContext db) => _db = db;

    public async Task<IReadOnlyList<SubAccountListItem>> GetByOrganizationIdAsync(Guid organizationId, CancellationToken ct)
    {
        var subAccounts = await _db.SubAccounts
            .Where(x => x.OrganizationId == organizationId)
            .ToListAsync(ct);

        var userIds = subAccounts.Select(x => x.UserId).ToList();

        var emailsByUserId = await _db.Users
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.Email ?? string.Empty, ct);

        return subAccounts.Select(sa => new SubAccountListItem(
            sa.Id,
            sa.Name,
            emailsByUserId.GetValueOrDefault(sa.UserId, string.Empty),
            sa.Status.ToString(),
            BuildScopeDescriptions(sa)
        )).ToList();
    }

    private static IReadOnlyList<string> BuildScopeDescriptions(SubAccount subAccount)
    {
        if (subAccount.ScopeType == ScopeType.Full)
            return new[] { "All Categories - All Services - All Types" };

        if (subAccount.Scopes.Count == 0)
            return Array.Empty<string>();

        return subAccount.Scopes
            .Select(s => s.Category == ScopeCategory.Financial
                ? "Financial"
                : $"{s.Category} - {s.Service} - {s.Type}")
            .ToList();
    }
}