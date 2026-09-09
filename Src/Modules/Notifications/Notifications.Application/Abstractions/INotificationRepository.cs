using Notifications.Domain.Notifications;

namespace Notifications.Application.Abstractions;

public interface INotificationRepository
{
    Task AddAsync(Notification notification, CancellationToken ct = default);

    Task<IReadOnlyList<Notification>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);

    Task<Notification?> GetByIdAsync(Guid notificationId, Guid userId, CancellationToken ct = default);



}

