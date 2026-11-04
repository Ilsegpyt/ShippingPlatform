using Customers.Contracts;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Repositories;
using Identity.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace Identity.Infrastructure.Authorization;

public sealed class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    private readonly ISubAccountRepository _subAccounts;
    private readonly IInternalUserRepository _internalUsers;
    private readonly IRoleRepository _roles;
    private readonly ICustomerQueries _customerQueries;
    private readonly IMemoryCache _cache;

    public PermissionAuthorizationHandler(
        ISubAccountRepository subAccounts,
        IInternalUserRepository internalUsers,
        IRoleRepository roles,
        ICustomerQueries customerQueries,
        IMemoryCache cache)
    {
        _subAccounts = subAccounts;
        _internalUsers = internalUsers;
        _roles = roles;
        _customerQueries = customerQueries;
        _cache = cache;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var userIdClaim =
            context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? context.User.FindFirstValue("sub");

        var tokenType = context.User.FindFirstValue("token_type");

        if (userIdClaim is null ||
            !Guid.TryParse(userIdClaim, out var userId))
        {
            return;
        }

        var granted = tokenType switch
        {
            // Customer owner permissions.
            "customer" => await HasCustomerPermissionAsync(
                userId,
                requirement.Permission),

            // Impersonation permissions.
            "impersonation" => await HasImpersonationPermissionAsync(
                context.User,
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
    /// Checks whether the customer owner has the required permission and is active.
    /// Uses memory caching (60 seconds) to avoid database pressure on every request.
    /// </summary>
    private async Task<bool> HasCustomerPermissionAsync(
        Guid userId,
        PermissionKey permission)
    {
        if (!PermissionCatalog.CustomerOwnerPermissions.Contains(permission))
            return false;

        var cacheKey = $"customer_active_user_{userId}";
        if (!_cache.TryGetValue(cacheKey, out bool isActive))
        {
            var customer = await _customerQueries.GetByUserIdAsync(userId, default);
            isActive = customer is not null && customer.IsActive && !customer.IsDeleted;
            _cache.Set(cacheKey, isActive, TimeSpan.FromMinutes(5));
        }

        return isActive;
    }

    /// <summary>
    /// Checks permissions available specifically during impersonation.
    /// Impersonation keeps the customer's normal permissions and adds
    /// impersonation-specific permissions, while verifying the customer is active.
    /// </summary>
    private async Task<bool> HasImpersonationPermissionAsync(
        ClaimsPrincipal user,
        PermissionKey permission)
    {
        var hasBasePermission =
            PermissionCatalog.CustomerOwnerPermissions.Contains(permission)
            || permission == PermissionCatalog.ImpersonatedCustomerView;

        if (!hasBasePermission)
            return false;

        var orgIdClaim = user.FindFirstValue("org_id");
        if (!Guid.TryParse(orgIdClaim, out var customerId))
            return false;

        var cacheKey = $"customer_active_id_{customerId}";
        if (!_cache.TryGetValue(cacheKey, out bool isActive))
        {
            var customer = await _customerQueries.GetByIdAsync(customerId, default);
            isActive = customer is not null && customer.IsActive && !customer.IsDeleted;
            _cache.Set(cacheKey, isActive, TimeSpan.FromMinutes(5));
        }

        return isActive;
    }

    /// <summary>
    /// Checks whether an active SubAccount has the required permission,
    /// and ensures the parent customer is also active.
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

        // Check whether parent customer is active
        var cacheKey = $"customer_active_id_{subAccount.CustomerId}";
        if (!_cache.TryGetValue(cacheKey, out bool isParentActive))
        {
            var parentCustomer = await _customerQueries.GetByIdAsync(subAccount.CustomerId, default);
            isParentActive = parentCustomer is not null && parentCustomer.IsActive && !parentCustomer.IsDeleted;
            _cache.Set(cacheKey, isParentActive, TimeSpan.FromMinutes(5));
        }

        if (!isParentActive)
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
