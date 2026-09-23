using Microsoft.EntityFrameworkCore;
using Notifications.Application.Abstractions;
using Notifications.Domain.Notifications;
using Notifications.Domain.Outbox;

namespace Notifications.Infrastructure.Persistence;

public sealed class NotificationsDbContext
    : DbContext, INotificationsUnitOfWork
{
    public NotificationsDbContext(
        DbContextOptions<NotificationsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Notification> Notifications => Set<Notification>();

    public DbSet<EmailOutboxMessage> EmailOutboxMessages => Set<EmailOutboxMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(NotificationsDbContext).Assembly);
    }
}