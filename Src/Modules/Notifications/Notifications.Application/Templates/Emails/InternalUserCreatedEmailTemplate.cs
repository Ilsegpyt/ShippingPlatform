namespace Notifications.Application.Templates.Emails;
using System.Net;

public static class InternalUserCreatedEmailTemplate
{
    public static string Build(
        string name,
        string email)
    {
        name = WebUtility.HtmlEncode(name);
        email = WebUtility.HtmlEncode(email);
        return $"""
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Welcome to ILS</title>
            </head>

            <body style="margin:0; padding:0; background-color:#f4f6f8; font-family:Arial, sans-serif;">

                <div style="max-width:600px; margin:40px auto; background:#ffffff; border-radius:8px; padding:40px;">

                    <h2 style="margin-top:0;">
                        Welcome to ILS
                    </h2>

                    <p>
                        Hello {name},
                    </p>

                    <p>
                        Your account has been created successfully.
                    </p>

                    <p>
                        <strong>Email:</strong> {email}
                    </p>

                    <p>
                        Welcome to ILS.
                    </p>

                </div>

            </body>
            </html>
            """;
    }
}