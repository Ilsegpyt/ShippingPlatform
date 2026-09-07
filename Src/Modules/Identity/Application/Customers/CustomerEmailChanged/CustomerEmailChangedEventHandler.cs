using BuildingBlocks.Contracts.IntegrationEvents.Customers;
using Identity.Application.Abstractions;
using MediatR;

namespace Identity.Application.Customers.CustomerEmailChanged;

public sealed class CustomerEmailChangedEventHandler(
    IIdentityUserService identityUserService)
    : INotificationHandler<CustomerEmailChangedIntegrationEvent>
{
    public async Task Handle(CustomerEmailChangedIntegrationEvent notification, CancellationToken ct)
    {
        await identityUserService.UpdateEmailAsync(notification.OwnerUserId, notification.NewEmail, ct);
    }
}