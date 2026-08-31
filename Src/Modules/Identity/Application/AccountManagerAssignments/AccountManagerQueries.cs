using BuildingBlocks.Application.Contracts;
using Identity.Domain.Repositories;

namespace Identity.Application.AccountManagerAssignments;

public sealed class AccountManagerQueries(
    IAccountManagerAssignmentRepository repository,
    IInternalUserRepository internalUsers)
    : IAccountManagerQueries
{
    public async Task<bool> IsAssignedToCustomerAsync(
        Guid accountManagerUserId,
        Guid customerId,
        CancellationToken ct)
    {
        var internalUser =
            await internalUsers.GetByUserIdAsync(
                accountManagerUserId,
                ct);

        if (internalUser is null)
            return false;

        var assignment =
            await repository.GetByCustomerIdAsync(
                customerId,
                ct);

        return assignment is not null
            && assignment.AccountManagerId == internalUser.Id;
    }
}