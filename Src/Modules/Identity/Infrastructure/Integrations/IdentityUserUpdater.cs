using BuildingBlocks.Application;
using Identity.Application.Abstractions;
using Identity.Contracts;

namespace Identity.Infrastructure.Integrations;

internal sealed class IdentityUserUpdater(IIdentityUserService identityUsers) : IIdentityUserUpdater
{
    public async Task<Result> UpdateEmailAsync(Guid userId, string email, CancellationToken ct = default)
    {
        var result = await identityUsers.UpdateEmailAsync(userId, email, ct);

        if (!result.Succeeded)
            return Result.Failure(result.Error!);

        return Result.Success();
    }
}