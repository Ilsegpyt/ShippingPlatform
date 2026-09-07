using MediatR;

namespace BuildingBlocks.Contracts.IntegrationEvents.Customers;

public sealed record CustomerEmailChangedIntegrationEvent(
    Guid CustomerId,
    Guid OwnerUserId,
    string NewEmail) : INotification;