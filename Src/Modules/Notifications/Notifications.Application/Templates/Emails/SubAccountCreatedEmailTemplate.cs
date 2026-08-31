namespace Notifications.Application.Templates.Emails;

public static class SubAccountCreatedEmailTemplate
{
    public static string Build(
        string name,
        string email)
    {
        return $"""
            <html>
            <body>
                <h2>Welcome to ILS, {name}!</h2>

                <p>
                    Your sub-account has been created successfully.
                </p>

                <p>
                    Email: {email}
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
