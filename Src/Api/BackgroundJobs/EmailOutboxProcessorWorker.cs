using Microsoft.EntityFrameworkCore;
using Notifications.Application.Abstractions;
using Notifications.Infrastructure.Persistence;

namespace Api.BackgroundJobs;

public sealed class EmailOutboxProcessorWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<EmailOutboxProcessorWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();

                var dbContext = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
                var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

                var messages = await dbContext.EmailOutboxMessages
                    .Where(x => x.SentAtUtc == null && x.RetryCount < 5)
                    .OrderBy(x => x.CreatedAtUtc)
                    .Take(20)
                    .ToListAsync(stoppingToken);

                foreach (var message in messages)
                {
                    try
                    {
                        await emailSender.SendAsync(
                            message.RecipientEmail,
                            message.Subject,
                            message.Body,
                            stoppingToken);

                        message.MarkAsSent(DateTime.UtcNow);
                    }
                    catch (Exception ex)
                    {
                        message.MarkAsFailed(ex.Message);

                        logger.LogError(
                            ex,
                            "Failed to send email to {RecipientEmail}",
                            message.RecipientEmail);
                    }
                }

                await dbContext.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while processing email outbox");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}