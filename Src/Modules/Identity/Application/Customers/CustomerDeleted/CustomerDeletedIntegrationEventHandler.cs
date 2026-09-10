using BuildingBlocks.Contracts.IntegrationEvents.Customers;
using Identity.Application.Abstractions;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.Customers.CustomerDeleted;

public sealed class CustomerDeletedIntegrationEventHandler(ISubAccountRepository subAccountRepository, IIdentityUnitOfWork identityUnitOfWork)
    : INotificationHandler<CustomerDeletedIntegrationEvent>
{
    public async Task Handle(CustomerDeletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var subAccounts =
          await subAccountRepository.GetByCustomerIdAsync(notification.CustomerId, cancellationToken);

        foreach (var subAccount in subAccounts)
        {
            subAccount.Deactivate();
        }

        await identityUnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
