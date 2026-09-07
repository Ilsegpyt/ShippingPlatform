using Identity.Contracts;
using Identity.Domain.Repositories;

namespace Identity.Application.AccountManagerAssignments;

/// <summary>
/// Provides account manager assignment queries for other modules.
/// </summary>

public sealed class AccountManagerQueries(
    IAccountManagerAssignmentRepository accountManagerAssignmentRepository,
    IInternalUserRepository internalUsersRepository)
    : IAccountManagerQueries
{
    /// <summary>
    /// Checks whether an Account Manager is assigned to a specific Customer.
    /// </summary>
    public async Task<bool> IsAssignedToCustomerAsync(Guid accountManagerUserId, Guid customerId, CancellationToken ct)
    {

        var internalUser =
            await internalUsersRepository.GetByUserIdAsync(accountManagerUserId, ct);

        if (internalUser is null)
            return false;

        var assignment =
            await accountManagerAssignmentRepository.GetByCustomerIdAsync(customerId, ct);

        return assignment is not null
            && assignment.AccountManagerId == internalUser.Id;
    }
}