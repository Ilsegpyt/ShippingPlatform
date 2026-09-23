namespace Notifications.Domain.Notifications;

public sealed class Notification
{
    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public NotificationType Type { get; private set; }

    public string Title { get; private set; } = null!;

    public string Message { get; private set; } = null!;

    public Guid? ShipmentId { get; private set; }

    public Guid? CustomerVoiceId { get; private set; }

    public bool IsRead { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Notification()
    {
    }

    private Notification(
        Guid id,
        Guid userId,
        NotificationType type,
        string title,
        string message,
        Guid? shipmentId,
        Guid? customerVoiceId)
    {
        Id = id;
        UserId = userId;
        Type = type;
        Title = title;
        Message = message;
        ShipmentId = shipmentId;
        CustomerVoiceId = customerVoiceId;
        IsRead = false;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Notification Create(
        Guid userId,
        NotificationType type,
        string title,
        string message,
        Guid? shipmentId = null,
        Guid? customerVoiceId = null)
    {
        return new Notification(
            Guid.NewGuid(),
            userId,
            type,
            title,
            message,
            shipmentId,
            customerVoiceId);
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }
}

public enum NotificationType
{
    ShipmentStatusChanged = 0,
    CustomerVoiceCreated = 1
}