using MediatR;

namespace BuildingBlocks.Contracts.IntegrationEvents.Customers;

public sealed record CustomerDeletedIntegrationEvent(
    Guid CustomerId) : INotification;
