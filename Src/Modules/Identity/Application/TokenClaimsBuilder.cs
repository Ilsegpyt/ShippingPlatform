using BuildingBlocks.Application;
using BuildingBlocks.Application.Contracts;
using Identity.Domain;
using Identity.Domain.Repositories;

namespace Identity.Application;

public sealed class TokenClaimsBuilder(ICustomerQueries customerQueries, ISubAccountRepository subAccounts, IInternalUserRepository internalUsers)
{
    public async Task<Result<Dictionary<string, string>>> BuildAsync(Guid userId, CancellationToken ct)
    {
        var customer = await customerQueries.GetByOwnerUserIdAsync(userId, ct);

        if (customer is not null)
        {
            if (!customer.IsActive)
                return Result.Failure<Dictionary<string, string>>("This account has been deactivated.");


            return Result.Success(new Dictionary<string, string>
            {
                ["sub"] = userId.ToString(),
                ["token_type"] = "customer",
                ["org_id"] = customer.CustomerId.ToString()
            });
        }

        var subAccount = await subAccounts.GetByUserIdAsync(userId, ct);

        if (subAccount is not null)
        {
            if (subAccount.Status != SubAccountStatus.Active)
                return Result.Failure<Dictionary<string, string>>("This account has been deactivated.");


            return Result.Success(new Dictionary<string, string>
            {
                ["sub"] = userId.ToString(),
                ["token_type"] = "subaccount",
                ["org_id"] = subAccount.OrganizationId.ToString()
            });
        }

        var internalUser = await internalUsers.GetByUserIdAsync(userId, ct);

        if (internalUser is null)
            return Result.Failure<Dictionary<string, string>>("No business profile is linked to this account.");


        if (internalUser.Status != InternalUserStatus.Active)
            return Result.Failure<Dictionary<string, string>>("This account has been deactivated.");


        return Result.Success(new Dictionary<string, string>
        {
            ["sub"] = userId.ToString(),
            ["token_type"] = "internal"
        });
    }
}