using Customers.Domain;
using Customers.Domain.Events;
using Identity.Application.Abstractions;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.Customers.CustomerStatusChanged;

public sealed class CustomerStatusChangedEventHandler(
    ISubAccountRepository subAccountRepository,
    IIdentityUnitOfWork identityUnitOfWork)
    : INotificationHandler<CustomerStatusChangedEvent>
{
    public async Task Handle(CustomerStatusChangedEvent notification, CancellationToken ct)
    {
        var subAccounts =
            await subAccountRepository.GetByOrganizationIdAsync(notification.CustomerId, ct);

        foreach (var subAccount in subAccounts)
        {
            if (notification.NewStatus == CustomerStatus.Suspended)
                subAccount.Deactivate();

            else if (notification.NewStatus == CustomerStatus.Active)
                subAccount.Activate();
        }

        await identityUnitOfWork.SaveChangesAsync(ct);
    }
}