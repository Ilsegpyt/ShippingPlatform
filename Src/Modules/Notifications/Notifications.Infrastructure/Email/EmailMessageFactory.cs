using MimeKit;

namespace Notifications.Infrastructure.Email;

public static class EmailMessageFactory
{
    public static MimeMessage Create(
        EmailOptions options,
        string recipientEmail,
        string subject,
        string body)
    {
        var message = new MimeMessage();

        message.From.Add(
            new MailboxAddress(
                options.FromName,
                options.FromEmail));

        message.To.Add(
            MailboxAddress.Parse(recipientEmail));

        message.Subject = subject;

        message.Body = new TextPart("html")
        {
            Text = body
        };

        return message;
    }
}