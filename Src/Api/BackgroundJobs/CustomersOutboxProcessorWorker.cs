
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
                        x.Type ==
                        typeof(CustomerRegisteredEvent)
                            .AssemblyQualifiedName)
                    .OrderBy(x => x.OccurredOnUtc)
                    .Take(20)
                    .ToListAsync(stoppingToken);

                foreach (var message in messages)
                {
                    try
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

                        await publisher.Publish(
                            integrationEvent,
                            stoppingToken);

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
    }
}

