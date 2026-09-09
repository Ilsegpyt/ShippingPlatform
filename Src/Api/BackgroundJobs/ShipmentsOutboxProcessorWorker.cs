using BuildingBlocks.Contracts.IntegrationEvents.Shipments;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Shipments.Domain.Events;
using Shipments.Infrastructure.Persistence;
using System.Text.Json;

namespace Api.BackgroundJobs;

public sealed class ShipmentsOutboxProcessorWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ShipmentsOutboxProcessorWorker> _logger;

    public ShipmentsOutboxProcessorWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<ShipmentsOutboxProcessorWorker> logger)
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
                    .GetRequiredService<ShipmentsDbContext>();

                var publisher = scope.ServiceProvider
                    .GetRequiredService<IPublisher>();

                var messages = await dbContext.OutboxMessages
                    .Where(x =>
                        x.ProcessedOnUtc == null &&
                        x.RetryCount < 5 &&
                        x.Type ==
                        typeof(ShipmentStatusChangedDomainEvent)
                            .AssemblyQualifiedName)
                    .OrderBy(x => x.OccurredOnUtc)
                    .Take(20)
                    .ToListAsync(stoppingToken);

                foreach (var message in messages)
                {
                    try
                    {
                        var domainEvent =
                            JsonSerializer.Deserialize<ShipmentStatusChangedDomainEvent>(
                                message.Payload);

                        if (domainEvent is null)
                        {
                            message.MarkAsFailed(
                                "Failed to deserialize ShipmentStatusChangedDomainEvent.");

                            continue;
                        }

                        var integrationEvent =
                            new ShipmentStatusChangedIntegrationEvent(
                                    domainEvent.ShipmentId,
                                    domainEvent.ShipmentRef,
                                    domainEvent.CustomerId,
                                    domainEvent.OldStatus.ToString(),
                                    domainEvent.NewStatus.ToString(),
                                    domainEvent.OccurredOnUtc);

                        await publisher.Publish(
                            integrationEvent,
                            stoppingToken);

                        message.MarkAsProcessed(DateTime.UtcNow);

                        _logger.LogInformation(
                            "Shipment outbox message {MessageId} processed successfully.",
                            message.Id);
                    }
                    catch (Exception ex)
                    {
                        message.MarkAsFailed(ex.Message);

                        _logger.LogError(
                            ex,
                            "Failed to process shipment outbox message {MessageId}. Retry count: {RetryCount}",
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
                    "Error while processing shipment outbox messages.");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(10),
                stoppingToken);
        }
    }
}