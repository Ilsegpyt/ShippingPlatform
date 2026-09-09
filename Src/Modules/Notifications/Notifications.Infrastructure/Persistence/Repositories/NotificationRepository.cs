using Microsoft.EntityFrameworkCore;
using Notifications.Application.Abstractions;
using Notifications.Domain.Notifications;

namespace Notifications.Infrastructure.Persistence.Repositories;

public sealed class NotificationRepository(
    NotificationsDbContext dbContext) : INotificationRepository
{
    public async Task AddAsync(
        Notification notification,
        CancellationToken ct = default)
    {
        await dbContext.Notifications.AddAsync(notification, ct);
    }
    public async Task<IReadOnlyList<Notification>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await dbContext.Notifications
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(ct);
    }
}