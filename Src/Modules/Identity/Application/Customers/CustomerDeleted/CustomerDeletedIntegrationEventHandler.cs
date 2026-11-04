using BuildingBlocks.Contracts.IntegrationEvents.Customers;
using Customers.Contracts;
using Identity.Application.Abstractions;
using Identity.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Identity.Application.Customers.CustomerDeleted;

public sealed class CustomerDeletedIntegrationEventHandler(
    ISubAccountRepository subAccountRepository,
    IIdentityUnitOfWork identityUnitOfWork,
    ICustomerQueries customerQueries,
    IIdentityUserService identityUsers,
    IMemoryCache cache)
    : INotificationHandler<CustomerDeletedIntegrationEvent>
{
    public async Task Handle(CustomerDeletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        // 1. Deactivate sub-accounts
        var subAccounts =
            await subAccountRepository.GetByCustomerIdAsync(notification.CustomerId, cancellationToken);

        foreach (var subAccount in subAccounts)
        {
            subAccount.Deactivate();
        }

        // 2. Deactivate customer owner's identity user
        var owner = await customerQueries.GetOwnerByCustomerIdAsync(
            notification.CustomerId,
            cancellationToken);

        if (owner is not null)
        {
            await identityUsers.SetUserStatusAsync(
                owner.OwnerUserId,
                false,
                cancellationToken);

            cache.Remove($"customer_active_user_{owner.OwnerUserId}");
            cache.Set($"customer_active_user_{owner.OwnerUserId}", false, TimeSpan.FromMinutes(5));
        }

        // 3. Invalidate/update cache by customer id
        cache.Remove($"customer_active_id_{notification.CustomerId}");
        cache.Set($"customer_active_id_{notification.CustomerId}", false, TimeSpan.FromMinutes(5));

        await identityUnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
