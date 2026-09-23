using Customers.Contracts;
using Notifications.Application.Abstractions;
using Notifications.Domain.Notifications;

namespace Notifications.Application.Services;

public sealed class NotificationQueries(
    INotificationRepository repository,
    INotificationsUnitOfWork unitOfWork,
    ICustomerQueries customerQueries)
{
    public async Task<IReadOnlyList<Notification>> GetByUserIdAsync(
        Guid userId,
        string? tokenType,
        string? organizationId,
        CancellationToken ct = default)
    {
        var targetUserId = await ResolveTargetUserIdAsync(
            userId,
            tokenType,
            organizationId,
            ct);

        return await repository.GetByUserIdAsync(
            targetUserId,
            ct);
    }

    public async Task<bool> MarkAsReadAsync(
        Guid notificationId,
        Guid userId,
        string? tokenType,
        string? organizationId,
        CancellationToken ct = default)
    {
        var targetUserId = await ResolveTargetUserIdAsync(
            userId,
            tokenType,
            organizationId,
            ct);

        var notification =
            await repository.GetByIdAsync(
                notificationId,
                targetUserId,
                ct);

        if (notification is null)
            return false;

        notification.MarkAsRead();

        await unitOfWork.SaveChangesAsync(ct);

        return true;
    }

    private async Task<Guid> ResolveTargetUserIdAsync(
        Guid userId,
        string? tokenType,
        string? organizationId,
        CancellationToken ct)
    {
        if (tokenType == "impersonation" &&
            Guid.TryParse(
                organizationId,
                out var impersonatedCustomerId))
        {
            var owner =
                await customerQueries.GetOwnerByCustomerIdAsync(
                    impersonatedCustomerId,
                    ct);

            if (owner is not null)
                return owner.OwnerUserId;
        }

        return userId;
    }
}