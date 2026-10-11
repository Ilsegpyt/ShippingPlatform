
using BuildingBlocks.Contracts.IntegrationEvents.Customers;
using Identity.Application.Abstractions;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.Customers.CustomerStatusChanged;

public sealed class CustomerStatusChangedEventHandler(
    ISubAccountRepository subAccountRepository,
    IIdentityUnitOfWork identityUnitOfWork)
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

        await identityUnitOfWork.SaveChangesAsync(ct);
    }
}
