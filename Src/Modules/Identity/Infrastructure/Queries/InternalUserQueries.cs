using Identity.Contracts;
using Identity.Domain.Repositories;

namespace Identity.Infrastructure.Queries;

public sealed class InternalUserQueries(
    IInternalUserRepository internalUserRepository)
    : IInternalUserQueries
{
    public async Task<string?> GetNameByUserIdAsync(
        Guid userId,
        CancellationToken ct)
    {
        var user = await internalUserRepository
            .GetByUserIdAsync(userId, ct);

        return user?.Name;
    }
}