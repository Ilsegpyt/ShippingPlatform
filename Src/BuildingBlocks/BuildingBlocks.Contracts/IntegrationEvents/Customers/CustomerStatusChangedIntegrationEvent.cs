using MediatR;

namespace BuildingBlocks.Contracts.IntegrationEvents.Customers;

public sealed record CustomerStatusChangedIntegrationEvent(
    Guid CustomerId,
    string NewStatus) : INotification;