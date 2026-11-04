using BuildingBlocks.Contracts.IntegrationEvents.Customers;
using Customers.Contracts;
using Identity.Application.Abstractions;
using Identity.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Identity.Application.Customers.CustomerStatusChanged;

public sealed class CustomerStatusChangedEventHandler(
    ISubAccountRepository subAccountRepository,
    IIdentityUnitOfWork identityUnitOfWork,
    ICustomerQueries customerQueries,
    IIdentityUserService identityUsers,
    IMemoryCache cache)
    : INotificationHandler<CustomerStatusChangedIntegrationEvent>
{
    public async Task Handle(
        CustomerStatusChangedIntegrationEvent notification,
        CancellationToken ct)
    {
        var shouldActivate = notification.NewStatus switch
        {
            "Active" => true,
            "Suspended" => false,
            _ => (bool?)null
        };

        if (shouldActivate is null)
            return;

        // 1. Activate or deactivate all sub-accounts
        var subAccounts = await subAccountRepository.GetByCustomerIdAsync(
            notification.CustomerId,
            ct);

        foreach (var subAccount in subAccounts)
        {
            if (shouldActivate.Value)
                subAccount.Activate();
            else
                subAccount.Deactivate();
        }

        // 2. Activate or deactivate the Customer Owner's Identity User
        var owner = await customerQueries.GetOwnerByCustomerIdAsync(
            notification.CustomerId,
            ct);

        if (owner is not null)
        {
            await identityUsers.SetUserStatusAsync(
                owner.OwnerUserId,
                shouldActivate.Value,
                ct);

            // Invalidate/update cache by user id
            cache.Remove($"customer_active_user_{owner.OwnerUserId}");
            cache.Set($"customer_active_user_{owner.OwnerUserId}", shouldActivate.Value, TimeSpan.FromMinutes(5));
        }

        // 3. Invalidate/update cache by customer id
        cache.Remove($"customer_active_id_{notification.CustomerId}");
        cache.Set($"customer_active_id_{notification.CustomerId}", shouldActivate.Value, TimeSpan.FromMinutes(5));

        await identityUnitOfWork.SaveChangesAsync(ct);
    }
}
