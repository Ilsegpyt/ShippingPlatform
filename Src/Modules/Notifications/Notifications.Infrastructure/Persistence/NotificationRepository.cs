using Notifications.Application.Abstractions;
using Notifications.Domain.Notifications;

namespace Notifications.Infrastructure.Persistence;

public sealed class NotificationRepository(
    NotificationsDbContext dbContext) : INotificationRepository
{
    public async Task AddAsync(
        Notification notification,
        CancellationToken ct = default)
    {
        await dbContext.Notifications.AddAsync(notification, ct);
    }
}