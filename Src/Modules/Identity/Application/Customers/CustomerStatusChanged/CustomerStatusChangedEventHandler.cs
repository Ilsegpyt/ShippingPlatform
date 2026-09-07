using BuildingBlocks.Contracts.IntegrationEvents.Customers;
using Identity.Application.Abstractions;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.Customers.CustomerStatusChanged;

public sealed class CustomerStatusChangedEventHandler(ISubAccountRepository subAccountRepository, IIdentityUnitOfWork identityUnitOfWork)
    : INotificationHandler<CustomerStatusChangedIntegrationEvent>
{
    public async Task Handle(CustomerStatusChangedIntegrationEvent notification, CancellationToken ct)
    {
        var subAccounts =
            await subAccountRepository.GetByOrganizationIdAsync(notification.CustomerId, ct);


        foreach (var subAccount in subAccounts)
        {
            if (notification.NewStatus == "Suspended")
                subAccount.Deactivate();

            else if (notification.NewStatus == "Active")
                subAccount.Activate();
        }

        await identityUnitOfWork.SaveChangesAsync(ct);
    }
}












