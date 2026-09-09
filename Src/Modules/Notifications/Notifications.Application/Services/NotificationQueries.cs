using Notifications.Application.Abstractions;
using Notifications.Domain.Notifications;

namespace Notifications.Application.Services;

public sealed class NotificationQueries(INotificationRepository repository)
{
    public async Task<IReadOnlyList<Notification>> GetByUserIdAsync(
        Guid userId,
        CancellationToken ct = default)
    {
        return await repository.GetByUserIdAsync(userId, ct);
    }
}