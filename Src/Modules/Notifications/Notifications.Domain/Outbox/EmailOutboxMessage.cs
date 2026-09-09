namespace Notifications.Domain.Outbox;

public sealed class EmailOutboxMessage
{
    public Guid Id { get; private set; }

    public string RecipientEmail { get; private set; } = null!;

    public string Subject { get; private set; } = null!;

    public string Body { get; private set; } = null!;

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? SentAtUtc { get; private set; }

    public string? Error { get; private set; }

    public int RetryCount { get; private set; }

    private EmailOutboxMessage()
    {
    }

    public EmailOutboxMessage(
        Guid id,
        string recipientEmail,
        string subject,
        string body,
        DateTime createdAtUtc)
    {
        Id = id;
        RecipientEmail = recipientEmail;
        Subject = subject;
        Body = body;
        CreatedAtUtc = createdAtUtc;
    }

    public void MarkAsSent(DateTime sentAtUtc)
    {
        SentAtUtc = sentAtUtc;
        Error = null;
    }

    public void MarkAsFailed(string error)
    {
        RetryCount++;
        Error = error;
    }
}