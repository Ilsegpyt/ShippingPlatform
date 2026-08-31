
using Identity.Domain;
using Identity.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Identity.Infrastructure.Authorization;

public sealed class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    private readonly ISubAccountRepository _subAccounts;
    private readonly IInternalUserRepository _internalUsers;
    private readonly IRoleRepository _roles;

    public PermissionAuthorizationHandler(
        ISubAccountRepository subAccounts,
        IInternalUserRepository internalUsers,
        IRoleRepository roles)
    {
        _subAccounts = subAccounts;
        _internalUsers = internalUsers;
        _roles = roles;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Get the authenticated user's ID from the JWT.
        var userIdClaim =
            context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? context.User.FindFirstValue("sub");

        // Determine the type of authenticated account.
        var tokenType = context.User.FindFirstValue("token_type");

        // The requirement cannot be satisfied without a valid user ID.
        if (userIdClaim is null ||
            !Guid.TryParse(userIdClaim, out var userId))
        {
            return;
        }

        var granted = tokenType switch
        {
            // Customer owner permissions.
            "customer" => HasCustomerPermission(
                requirement.Permission),

            // SubAccount permissions.
            "subaccount" => await HasSubAccountPermissionAsync(
                userId,
                requirement.Permission),

            // Internal staff permissions through their role.
            "internal" => await HasInternalPermissionAsync(
                userId,
                requirement.Permission),

            _ => false
        };

        if (granted)
            context.Succeed(requirement);
    }

    /// <summary>
    /// Checks whether the customer owner has the required permission.
    /// Customer owner permissions are defined as a fixed set.
    /// </summary>
    private static bool HasCustomerPermission(
        PermissionKey permission)
    {
        return PermissionCatalog.CustomerOwnerPermissions.Contains(permission);
    }

    /// <summary>
    /// Checks whether an active SubAccount has the required permission.
    /// </summary>
    private async Task<bool> HasSubAccountPermissionAsync(
        Guid userId,
        PermissionKey permission)
    {
        var subAccount = await _subAccounts.GetByUserIdAsync(userId);

        if (subAccount is null ||
            subAccount.Status != SubAccountStatus.Active)
        {
            return false;
        }

        return subAccount.HasPermission(permission);
    }

    /// <summary>
    /// Checks whether an active internal user has the required permission
    /// through their active role.
    /// </summary>
    private async Task<bool> HasInternalPermissionAsync(
        Guid userId,
        PermissionKey permission)
    {
        var internalUser = await _internalUsers.GetByUserIdAsync(userId);

        if (internalUser is null ||
            internalUser.Status != InternalUserStatus.Active)
        {
            return false;
        }

        var role = await _roles.GetByIdAsync(internalUser.RoleId);

        if (role is null ||
            role.Status != RoleStatus.Active)
        {
            return false;
        }

        return role.HasPermission(permission);
    }
}

