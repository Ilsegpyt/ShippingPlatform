using Customers.Domain.Events;
using Identity.Application.Abstractions;
using MediatR;

namespace Identity.Application.Customers.CustomerEmailChanged;

public sealed class CustomerEmailChangedEventHandler(
    IIdentityUserService identityUserService)
    : INotificationHandler<CustomerEmailChangedEvent>
{
    public async Task Handle(
        CustomerEmailChangedEvent notification,
        CancellationToken ct)
    {
        await identityUserService.UpdateEmailAsync(
            notification.OwnerUserId,
            notification.NewEmail,
            ct);
    }
}