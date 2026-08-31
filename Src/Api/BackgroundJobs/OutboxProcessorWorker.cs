using BuildingBlocks.Contracts.IntegrationEvents.Identity;
using Identity.Domain.Events;
using Identity.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Api.BackgroundJobs;

public sealed class OutboxProcessorWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessorWorker> _logger;

    public OutboxProcessorWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<OutboxProcessorWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var dbContext = scope.ServiceProvider
                    .GetRequiredService<IdentityDbContext>();

                var publisher = scope.ServiceProvider
                    .GetRequiredService<IPublisher>();

                var messages = await dbContext.OutboxMessages
                    .Where(x =>
                        x.ProcessedOnUtc == null &&
                        x.RetryCount < 5)
                    .OrderBy(x => x.OccurredOnUtc)
                    .Take(20)
                    .ToListAsync(stoppingToken);

                foreach (var message in messages)
                {
                    try
                    {
                        if (message.Type ==
                            typeof(InternalUserCreatedDomainEvent)
                                .AssemblyQualifiedName)
                        {
                            var domainEvent =
                                JsonSerializer.Deserialize<InternalUserCreatedDomainEvent>(
                                    message.Payload);

                            if (domainEvent is null)
                            {
                                message.MarkAsFailed(
                                    "Failed to deserialize InternalUserCreatedDomainEvent.");

                                continue;
                            }

                            var integrationEvent =
                                new InternalUserCreatedIntegrationEvent(
                                    domainEvent.UserId,
                                    domainEvent.Name,
                                    domainEvent.Email);

                            await publisher.Publish(
                                integrationEvent,
                                stoppingToken);

                            message.MarkAsProcessed(DateTime.UtcNow);

                            _logger.LogInformation(
                                "InternalUserCreated outbox message {MessageId} processed successfully.",
                                message.Id);
                        }
                        else if (message.Type ==
                                 typeof(SubAccountCreatedEvent)
                                     .AssemblyQualifiedName)
                        {
                            var domainEvent =
                                JsonSerializer.Deserialize<SubAccountCreatedEvent>(
                                    message.Payload);

                            if (domainEvent is null)
                            {
                                message.MarkAsFailed(
                                    "Failed to deserialize SubAccountCreatedEvent.");

                                continue;
                            }

                            var integrationEvent =
                                new SubAccountCreatedIntegrationEvent(
                                    domainEvent.SubAccountId,
                                    domainEvent.Name,
                                    domainEvent.Email);

                            await publisher.Publish(
                                integrationEvent,
                                stoppingToken);

                            message.MarkAsProcessed(DateTime.UtcNow);

                            _logger.LogInformation(
                                "SubAccountCreated outbox message {MessageId} processed successfully.",
                                message.Id);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        message.MarkAsFailed(ex.Message);

                        _logger.LogError(
                            ex,
                            "Failed to process outbox message {MessageId}. Retry count: {RetryCount}",
                            message.Id,
                            message.RetryCount);
                    }
                }

                await dbContext.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while processing outbox messages.");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(10),
                stoppingToken);
        }
    }
}
