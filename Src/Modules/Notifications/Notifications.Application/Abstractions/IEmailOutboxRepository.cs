using Notifications.Domain.Outbox;

namespace Notifications.Application.Abstractions;

public interface IEmailOutboxRepository
{
    Task AddAsync(
        EmailOutboxMessage message,
        CancellationToken ct = default);
}