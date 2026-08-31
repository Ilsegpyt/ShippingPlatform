using MediatR;

namespace BuildingBlocks.Contracts.IntegrationEvents.Customers;

public sealed record CustomerRegisteredIntegrationEvent(
    Guid CustomerId,
    Guid OwnerUserId,
    string OwnerName,
    string OwnerEmail) : INotification;