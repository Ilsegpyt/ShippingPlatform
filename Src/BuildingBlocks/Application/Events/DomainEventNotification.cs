using BuildingBlocks.Domain;
using MediatR;

namespace BuildingBlocks.Application.Events;

public sealed record DomainEventNotification(
    IDomainEvent DomainEvent) : INotification;