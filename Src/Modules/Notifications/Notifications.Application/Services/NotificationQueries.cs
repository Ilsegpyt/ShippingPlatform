using Notifications.Application.Abstractions;
using Notifications.Domain.Notifications;

namespace Notifications.Application.Services;

public sealed class NotificationQueries(INotificationRepository repository, INotificationsUnitOfWork unitOfWork)
{
    public async Task<IReadOnlyList<Notification>> GetByUserIdAsync(
        Guid userId,
        CancellationToken ct = default)
    {
        return await repository.GetByUserIdAsync(userId, ct);
    }
    public async Task<bool> MarkAsReadAsync(  Guid notificationId, Guid userId, CancellationToken ct = default)
    {
        var notification = await repository.GetByIdAsync(notificationId, userId, ct);

        if (notification is null)
            return false;

        notification.MarkAsRead();
        await unitOfWork.SaveChangesAsync(ct);


        return true;
    }
}