namespace Notifications.Domain.Notifications;

public sealed class Notification
{
    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public string Title { get; private set; } = null!;

    public string Message { get; private set; } = null!;

    public bool IsRead { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Notification()
    {
    }

    private Notification(
        Guid id,
        Guid userId,
        string title,
        string message)
    {
        Id = id;
        UserId = userId;
        Title = title;
        Message = message;
        IsRead = false;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Notification Create(
        Guid userId,
        string title,
        string message)
    {
        return new Notification(
            Guid.NewGuid(),
            userId,
            title,
            message);
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }
}
