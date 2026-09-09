using Notifications.Application.Abstractions;
using Notifications.Domain.Outbox;

namespace Notifications.Infrastructure.Persistence.Repositories;

public sealed class EmailOutboxRepository(
    NotificationsDbContext dbContext) : IEmailOutboxRepository
{
    public async Task AddAsync(
        EmailOutboxMessage message,
        CancellationToken ct = default)
    {
        await dbContext.EmailOutboxMessages.AddAsync(message, ct);
    }
}