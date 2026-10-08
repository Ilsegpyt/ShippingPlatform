using BuildingBlocks.Contracts.IntegrationEvents.Customers;
using Customers.Domain.Events;
using Customers.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Api.BackgroundJobs;

public sealed class CustomersOutboxProcessorWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CustomersOutboxProcessorWorker> _logger;

    public CustomersOutboxProcessorWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<CustomersOutboxProcessorWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Customers Outbox Processor Worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var dbContext = scope.ServiceProvider
                    .GetRequiredService<CustomersDbContext>();

                var publisher = scope.ServiceProvider
                    .GetRequiredService<IPublisher>();

                var messages = await dbContext.OutboxMessages
                    .Where(x =>
                        x.ProcessedOnUtc == null &&
                        x.RetryCount < 5 &&
                        (
                            x.Type ==
                            typeof(CustomerRegisteredEvent)
                                .AssemblyQualifiedName
                            ||
                            x.Type ==
                            typeof(CustomerEmailChangedEvent)
                                .AssemblyQualifiedName
                            ||
                            x.Type ==
                            typeof(CustomerStatusChangedEvent)
                                .AssemblyQualifiedName
                            ||
                            x.Type ==
                            typeof(CustomerDeletedEvent)
                                .AssemblyQualifiedName
                            //||
                            //x.Type ==
                            //typeof(CustomerVoiceCreatedDomainEvent)
                            //    .AssemblyQualifiedName
                        ))
                    .OrderBy(x => x.OccurredOnUtc)
                    .Take(20)
                    .ToListAsync(stoppingToken);

                if (messages.Count > 0)
                {
                    _logger.LogInformation(
                        "Found {Count} customer outbox message(s) to process.",
                        messages.Count);
                }

                foreach (var message in messages)
                {
                    try
                    {
                        _logger.LogInformation(
                            "Processing customer outbox message {MessageId}. Type: {MessageType}",
                            message.Id,
                            message.Type);

                        if (message.Type ==
                            typeof(CustomerRegisteredEvent)
                                .AssemblyQualifiedName)
                        {
                            var domainEvent =
                                JsonSerializer.Deserialize<CustomerRegisteredEvent>(
                                    message.Payload);

                            if (domainEvent is null)
                            {
                                message.MarkAsFailed(
                                    "Failed to deserialize CustomerRegisteredEvent.");

                                continue;
                            }

                            var integrationEvent =
                                new CustomerRegisteredIntegrationEvent(
                                    domainEvent.CustomerId,
                                    domainEvent.OwnerUserId,
                                    domainEvent.OwnerName,
                                    domainEvent.OwnerEmail);

                            _logger.LogInformation(
                                "Publishing CustomerRegisteredIntegrationEvent for CustomerId {CustomerId}, UserId {UserId}, Email {Email}",
                                domainEvent.CustomerId,
                                domainEvent.OwnerUserId,
                                domainEvent.OwnerEmail);

                            await publisher.Publish(
                                integrationEvent,
                                stoppingToken);
                        }
                        else if (message.Type ==
                                 typeof(CustomerEmailChangedEvent)
                                     .AssemblyQualifiedName)
                        {
                            var domainEvent =
                                JsonSerializer.Deserialize<CustomerEmailChangedEvent>(
                                    message.Payload);

                            if (domainEvent is null)
                            {
                                message.MarkAsFailed(
                                    "Failed to deserialize CustomerEmailChangedEvent.");

                                continue;
                            }

                            var integrationEvent =
                                new CustomerEmailChangedIntegrationEvent(
                                    domainEvent.CustomerId,
                                    domainEvent.OwnerUserId,
                                    domainEvent.NewEmail);

                            await publisher.Publish(
                                integrationEvent,
                                stoppingToken);
                        }
                        else if (message.Type ==
                                 typeof(CustomerStatusChangedEvent)
                                     .AssemblyQualifiedName)
                        {
                            var domainEvent =
                                JsonSerializer.Deserialize<CustomerStatusChangedEvent>(
                                    message.Payload);

                            if (domainEvent is null)
                            {
                                message.MarkAsFailed(
                                    "Failed to deserialize CustomerStatusChangedEvent.");

                                continue;
                            }

                            var integrationEvent =
                                new CustomerStatusChangedIntegrationEvent(
                                    domainEvent.CustomerId,
                                    domainEvent.NewStatus.ToString());

                            await publisher.Publish(
                                integrationEvent,
                                stoppingToken);
                        }
                        else if (message.Type ==
                                 typeof(CustomerDeletedEvent)
                                     .AssemblyQualifiedName)
                        {
                            var domainEvent =
                                JsonSerializer.Deserialize<CustomerDeletedEvent>(
                                    message.Payload);

                            if (domainEvent is null)
                            {
                                message.MarkAsFailed(
                                    "Failed to deserialize CustomerDeletedEvent.");

                                continue;
                            }

                            var integrationEvent =
                                new CustomerDeletedIntegrationEvent(
                                    domainEvent.CustomerId);

                            await publisher.Publish(
                                integrationEvent,
                                stoppingToken);
                        }


                        message.MarkAsProcessed(DateTime.UtcNow);

                        _logger.LogInformation(
                            "Customer outbox message {MessageId} processed successfully.",
                            message.Id);
                    }
                    catch (Exception ex)
                    {
                        message.MarkAsFailed(ex.Message);

                        _logger.LogError(
                            ex,
                            "Failed to process customer outbox message {MessageId}. Retry count: {RetryCount}",
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
                    "Error while processing customer outbox messages.");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(10),
                stoppingToken);
        }

        _logger.LogInformation(
            "Customers Outbox Processor Worker stopped.");
    }
}