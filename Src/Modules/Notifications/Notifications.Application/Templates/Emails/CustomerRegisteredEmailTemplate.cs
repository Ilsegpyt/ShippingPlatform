namespace Notifications.Application.Templates.Emails;

public static class CustomerRegisteredEmailTemplate
{
    public static string Build(
        string ownerName,
        string ownerEmail)
    {
        return $"""
            <html>
            <body>
                <h2>Welcome to ILS, {ownerName}!</h2>

                <p>
                    Your customer account has been created successfully.
                </p>

                <p>
                    Email: {ownerEmail}
                </p>

                <p>
                    You can now log in to your ILS account.
                </p>

                <p>
                    Best regards,<br/>
                    ILS Egypt
                </p>
            </body>
            </html>
            """;
    }
}
