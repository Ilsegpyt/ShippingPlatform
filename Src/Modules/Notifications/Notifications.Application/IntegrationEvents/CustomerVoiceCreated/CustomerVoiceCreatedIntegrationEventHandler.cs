using BuildingBlocks.Contracts.IntegrationEvents.Customers;
using Identity.Contracts;
using MediatR;
using Notifications.Application.Abstractions;
using Notifications.Domain.Notifications;

namespace Notifications.Application.IntegrationEvents.CustomerVoiceCreated;

public sealed class CustomerVoiceCreatedIntegrationEventHandler(
    IUserAccessQueries userAccessQueries,
    INotificationRepository notificationRepository,
    INotificationsUnitOfWork notificationsUnitOfWork)
    : INotificationHandler<CustomerVoiceCreatedIntegrationEvent>
{
    public async Task Handle(
        CustomerVoiceCreatedIntegrationEvent notification,
        CancellationToken ct)
    {
        var superAdminUserIds =
            await userAccessQueries.GetSuperAdminUserIdsAsync(ct);

        if (superAdminUserIds.Count == 0)
            return;

        var title = "New Customer Voice";

        var message =
            $"A new customer voice was created: \"{notification.Subject}\".";

        foreach (var userId in superAdminUserIds)
        {
            var newNotification = Notification.Create(
                userId,
                NotificationType.CustomerVoiceCreated,
                title,
                message,
                notification.ShipmentId,
                notification.CustomerVoiceId);

            await notificationRepository.AddAsync(
                newNotification,
                ct);
        }

        await notificationsUnitOfWork.SaveChangesAsync(ct);
    }
}