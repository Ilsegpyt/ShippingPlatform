namespace Notifications.Application.Abstractions;

public interface INotificationsUnitOfWork
{
    Task<int> SaveChangesAsync(
        CancellationToken ct = default);
}