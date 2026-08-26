using BuildingBlocks.Application;
using BuildingBlocks.Application.Contracts;
using Identity.Application.Abstractions;

namespace Identity.Infrastructure.Integrations;

internal sealed class IdentityUserUpdater(
    IIdentityUserService identityUsers)
    : IIdentityUserUpdater
{
    public async Task<Result> UpdateEmailAsync(
        Guid userId,
        string email,
        CancellationToken ct = default)
    {
        return await identityUsers.UpdateEmailAsync(
            userId,
            email,
            ct);
    }
}