using Notifications.Domain.Notifications;

namespace Notifications.Application.Abstractions;

public interface INotificationRepository
{
    Task AddAsync(
        Notification notification,
        CancellationToken ct = default);
}