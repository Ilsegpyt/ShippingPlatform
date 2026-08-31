using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Notifications.Application.Abstractions;

namespace Notifications.Infrastructure.Email;

public sealed class EmailSender : IEmailSender
{
    private readonly EmailOptions _options;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(
        IOptions<EmailOptions> options,
        ILogger<EmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendAsync(
        string recipientEmail,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var message = EmailMessageFactory.Create(
                        _options,
                        recipientEmail,
                        subject,
                        body);

            using var smtp = new SmtpClient
            {
                Timeout = 60000
            };

            await smtp.ConnectAsync(
                _options.SmtpHost,
                _options.SmtpPort,
                SecureSocketOptions.StartTls,
                cancellationToken);

            await smtp.AuthenticateAsync(
                _options.Username,
                _options.Password,
                cancellationToken);

            await smtp.SendAsync(
                message,
                cancellationToken);

            _logger.LogInformation(
                "Email sent successfully to {RecipientEmail}. Subject: {Subject}",
                recipientEmail,
                subject);

            await smtp.DisconnectAsync(
                true,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send email to {RecipientEmail}. Subject: {Subject}",
                recipientEmail,
                subject);

            throw;
        }
    }
}