namespace Notifications.Application.Abstractions;

public interface IEmailSender
{
    Task SendAsync(
        string recipientEmail,
        string subject,
        string body,
        CancellationToken cancellationToken = default);
}