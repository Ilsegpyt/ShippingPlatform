using BuildingBlocks.Contracts.IntegrationEvents.Identity;
using Identity.Domain.Events;
using Identity.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Api.BackgroundJobs;

public sealed class IdentityOutboxProcessorWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<IdentityOutboxProcessorWorker> _logger;

    public IdentityOutboxProcessorWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<IdentityOutboxProcessorWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Identity Outbox Processor Worker started.");

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

                if (messages.Count > 0)
                {
                    _logger.LogInformation(
                        "Found {Count} identity outbox message(s) to process.",
                        messages.Count);
                }

                foreach (var message in messages)
                {
                    try
                    {
                        _logger.LogInformation(
                            "Processing identity outbox message {MessageId}. Type: {MessageType}",
                            message.Id,
                            message.Type);

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

                            _logger.LogInformation(
                                "Publishing InternalUserCreatedIntegrationEvent for UserId {UserId}, Email {Email}",
                                domainEvent.UserId,
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
                                    domainEvent.UserId,
                                    domainEvent.Name,
                                    domainEvent.Email);

                            _logger.LogInformation(
                                "Publishing SubAccountCreatedIntegrationEvent for SubAccountId {SubAccountId}, UserId {UserId}, Email {Email}",
                                domainEvent.SubAccountId,
                                domainEvent.UserId,
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
                            _logger.LogWarning(
                                "Unsupported identity outbox message {MessageId}. Type: {MessageType}",
                                message.Id,
                                message.Type);

                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        message.MarkAsFailed(ex.Message);

                        _logger.LogError(
                            ex,
                            "Failed to process identity outbox message {MessageId}. Retry count: {RetryCount}",
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
                    "Error while processing identity outbox messages.");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(10),
                stoppingToken);
        }

        _logger.LogInformation(
            "Identity Outbox Processor Worker stopped.");
    }
}