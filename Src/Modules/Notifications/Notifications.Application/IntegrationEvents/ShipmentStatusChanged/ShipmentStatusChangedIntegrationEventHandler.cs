using BuildingBlocks.Contracts.IntegrationEvents.Shipments;
using Customers.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;
using Notifications.Application.Abstractions;
using Notifications.Domain.Notifications;

namespace Notifications.Application.IntegrationEvents.ShipmentStatusChanged;

public sealed class ShipmentStatusChangedIntegrationEventHandler(
    ICustomerQueries customerQueries,
    INotificationsUnitOfWork notificationsUnitOfWork,
    IEmailSender emailSender,
    INotificationRepository notificationRepository,
    ILogger<ShipmentStatusChangedIntegrationEventHandler> logger)
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

        var title =
            $"Shipment {notification.ShipmentRef} - Status Update";

        var message =
            $"Your shipment {notification.ShipmentRef} status has been updated " +
            $"from \"{notification.OldStatus}\" to \"{notification.NewStatus}\".";

        var newNotification = Notification.Create(
            customer.OwnerUserId,
            title,
            message);

        await notificationRepository.AddAsync(
            newNotification,
            ct);

        await notificationsUnitOfWork.SaveChangesAsync(ct);

        try
        {
            await emailSender.SendAsync(
                customer.OwnerEmail,
                title,
                message,
                ct);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to send shipment status email for Shipment {ShipmentId} to {Email}",
                notification.ShipmentId,
                customer.OwnerEmail);
        }
    }
}