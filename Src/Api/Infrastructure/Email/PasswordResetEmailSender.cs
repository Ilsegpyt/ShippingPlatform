using Identity.Application.Abstractions;
using Notifications.Application.Abstractions;

namespace Api.Infrastructure.Email;

public sealed class PasswordResetEmailSender : IPasswordResetEmailSender
{
    private readonly IEmailSender _emailSender;

    public PasswordResetEmailSender(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public async Task SendAsync(
        string recipientEmail,
        string resetLink,
        CancellationToken cancellationToken = default)
    {
        const string subject = "Reset your password";

        var body = $"""
            Hello,

            We received a request to reset your password.

            Click the link below to reset your password:

            {resetLink}

            If you did not request a password reset, you can ignore this email.
            """;

        await _emailSender.SendAsync(
            recipientEmail,
            subject,
            body,
            cancellationToken);
    }
}