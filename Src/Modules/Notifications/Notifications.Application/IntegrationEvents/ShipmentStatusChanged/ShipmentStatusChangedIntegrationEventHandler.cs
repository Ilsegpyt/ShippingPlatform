using BuildingBlocks.Contracts.IntegrationEvents.Shipments;
using Customers.Contracts;
using MediatR;
using Notifications.Application.Abstractions;
using Notifications.Domain.Notifications;
using Notifications.Domain.Outbox;

namespace Notifications.Application.IntegrationEvents.ShipmentStatusChanged;

public sealed class ShipmentStatusChangedIntegrationEventHandler(
    ICustomerQueries customerQueries,
    INotificationsUnitOfWork notificationsUnitOfWork,
    IEmailOutboxRepository emailOutboxRepository,
    INotificationRepository notificationRepository)
    : INotificationHandler<ShipmentStatusChangedIntegrationEvent>
{
    public async Task Handle(
        ShipmentStatusChangedIntegrationEvent notification,
        CancellationToken ct)
    {
        var customer = await customerQueries.GetOwnerByCustomerIdAsync(
            notification.CustomerId,
            ct);

        if (customer is null)
            return;

        var title = $"Shipment {notification.ShipmentRef} - Status Update";

        var message =
            $"Your shipment {notification.ShipmentRef} status has been updated " +
            $"from \"{notification.OldStatus}\" to \"{notification.NewStatus}\".";

        var newNotification = Notification.Create(
            customer.OwnerUserId,
            title,
            message);

        var emailMessage = new EmailOutboxMessage(
            Guid.NewGuid(),
            customer.OwnerEmail,
            title,
            message,
            DateTime.UtcNow);

        await notificationRepository.AddAsync(newNotification, ct);
        await emailOutboxRepository.AddAsync(emailMessage, ct);
        await notificationsUnitOfWork.SaveChangesAsync(ct);
    }
}