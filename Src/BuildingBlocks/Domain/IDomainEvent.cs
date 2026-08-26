using MediatR;

namespace BuildingBlocks.Domain;

public interface IDomainEvent : INotification
{
    DateTime OccurredOnUtc { get; }
}