using BuildingBlocks.Domain;
using Customers.Domain.Events;

namespace Customers.Domain.Entities;

public sealed class CustomerVoice : AggregateRoot<Guid>
{
    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public Guid ShipmentId { get; private set; }

    public Guid UserId { get; private set; }

    public string Subject { get; private set; } = null!;

    public string Message { get; private set; } = null!;

    public CustomerVoiceStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    private CustomerVoice()
    {
    }

    private CustomerVoice(
        Guid id,
        Guid customerId,
        Guid shipmentId,
        Guid userId,
        string subject,
        string message)
    {
        Id = id;
        CustomerId = customerId;
        ShipmentId = shipmentId;
        UserId = userId;
        Subject = subject;
        Message = message;
        Status = CustomerVoiceStatus.Open;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public static CustomerVoice Create(
        Guid customerId,
        Guid shipmentId,
        Guid userId,
        string subject,
        string message)
    {
        var customerVoice = new CustomerVoice(
            Guid.NewGuid(),
            customerId,
            shipmentId,
            userId,
            subject,
            message);

        customerVoice.RaiseDomainEvent(
            new CustomerVoiceCreatedDomainEvent(
                customerVoice.Id,
                customerVoice.CustomerId,
                customerVoice.ShipmentId,
                customerVoice.UserId,
                customerVoice.Subject,
                customerVoice.CreatedAtUtc));

        return customerVoice;
    }

    public void ChangeStatus(CustomerVoiceStatus status)
    {
        Status = status;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}

public enum CustomerVoiceStatus
{
    Open = 0,
    InProgress = 1,
    Resolved = 2,
    Closed = 3
}