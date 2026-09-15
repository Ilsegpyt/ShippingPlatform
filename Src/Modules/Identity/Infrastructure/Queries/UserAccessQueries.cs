using Identity.Contracts;
using Identity.Domain.Repositories;

namespace Identity.Infrastructure.Queries;

public sealed class UserAccessQueries(
    IInternalUserRepository internalUserRepository,
    IRoleRepository roleRepository)
    : IUserAccessQueries
{
    public async Task<UserAccessInfo?> GetAccessInfoAsync(
        Guid userId,
        CancellationToken ct)
    {
        var internalUser =
            await internalUserRepository.GetByUserIdAsync(userId, ct);

        if (internalUser is null)
            return null;

        var role =
            await roleRepository.GetByIdAsync(
                internalUser.RoleId,
                ct);

        if (role is null)
        {
            return new UserAccessInfo(
                false,
                "internal",
                null,
                []);
        }

        return new UserAccessInfo(
            true,
            "internal",
            role.Name,
            []);
    }
}