using BuildingBlocks.Application.Contracts;
using Identity.Domain;
using Identity.Domain.Repositories;

namespace Identity.Infrastructure.Queries;

public sealed class SubAccountQueries(
    ISubAccountRepository subAccountRepository)
    : ISubAccountQueries
{
    public async Task<SubAccountAccessInfo?> GetAccessInfoAsync(
        Guid userId,
        CancellationToken ct)
    {
        var subAccount =
            await subAccountRepository.GetByUserIdAsync(userId, ct);

        if (subAccount is null)
            return null;

        return new SubAccountAccessInfo(
            subAccount.Id,
            subAccount.OrganizationId,
            subAccount.Status == SubAccountStatus.Active,
            subAccount.Permissions
                .Select(p => p.Value)
                .ToList(),
            subAccount.ScopeType == ScopeType.Full,
            subAccount.Scopes
                .Select(s => new SubAccountScopeInfo(
                    (int)s.Category,
                    (int)s.Service,
                    (int)s.Type))
                .ToList());
    }
}