namespace Identity.Application.Abstractions;

public interface IPasswordResetEmailSender
{
    Task SendAsync(
        string recipientEmail,
        string resetLink,
        CancellationToken cancellationToken = default);
}