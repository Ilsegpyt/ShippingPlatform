using BuildingBlocks.Application;
using Customers.Contracts;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Repositories;
using Identity.Domain.ValueObjects;
using MediatR;

namespace Identity.Application.Me.GetMe;

public sealed class GetMeQueryHandler(
    ICustomerQueries customerQueries,
    ISubAccountRepository subAccounts,
    IInternalUserRepository internalUsers,
    IRoleRepository roles)
    : IRequestHandler<GetMeQuery, Result<MeResponse>>
{
    public async Task<Result<MeResponse>> Handle(
        GetMeQuery query,
        CancellationToken ct)
    {
        var customer = await customerQueries.GetByUserIdAsync(
            query.UserId,
            ct);

        if (customer is not null)
        {
            if (!customer.IsActive)
            {
                return Result.Failure<MeResponse>(
                    "This account has been deactivated.");
            }

            var permissions = PermissionCatalog.CustomerOwnerPermissions
                .Select(x => x.Value)
                .ToList();

            return new MeResponse(
                query.UserId,
                "customer",
                customer.CustomerId,
                permissions);
        }

        var subAccount = await subAccounts.GetByUserIdAsync(
            query.UserId);

        if (subAccount is not null)
        {
            if (subAccount.Status != SubAccountStatus.Active)
            {
                return Result.Failure<MeResponse>(
                    "This account has been deactivated.");
            }

            var permissions = PermissionCatalog.All
                .Where(subAccount.HasPermission)
                .Select(x => x.Value)
                .ToList();

            return new MeResponse(
                query.UserId,
                "subaccount",
                subAccount.CustomerId,
                permissions);
        }

        var internalUser = await internalUsers.GetByUserIdAsync(
            query.UserId);

        if (internalUser is null)
        {
            return Result.Failure<MeResponse>(
                "No business profile is linked to this account.");
        }

        if (internalUser.Status != InternalUserStatus.Active)
        {
            return Result.Failure<MeResponse>(
                "This account has been deactivated.");
        }

        var role = await roles.GetByIdAsync(internalUser.RoleId, ct);

        if (role is null || role.Status != RoleStatus.Active)
        {
            return Result.Failure<MeResponse>(
                "User role is not available.");
        }

        var internalPermissions = PermissionCatalog.All
            .Where(role.HasPermission)
            .Select(x => x.Value)
            .ToList();

        return new MeResponse(
            query.UserId,
            "internal",
            null,
            internalPermissions);
    }
}