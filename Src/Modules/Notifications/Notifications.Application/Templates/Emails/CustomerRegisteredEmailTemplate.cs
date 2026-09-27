namespace Notifications.Application.Templates.Emails;

public static class CustomerRegisteredEmailTemplate
{
    public static string Build(
        string ownerName,
        string ownerEmail,
        string activationUrl)
    {
        return $"""
            <html>
            <body style="margin:0; padding:0; background-color:#f5f7fa; font-family:Arial, Helvetica, sans-serif; color:#333;">
                <div style="max-width:600px; margin:40px auto; background:#ffffff; border-radius:12px; padding:40px; box-shadow:0 2px 8px rgba(0,0,0,0.08);">

                    <h2 style="margin-top:0; color:#1f2937;">
                        Welcome to ILS, {ownerName}!
                    </h2>

                    <p style="font-size:15px; line-height:1.6;">
                        Your customer account has been created successfully.
                    </p>

                    <p style="font-size:15px; line-height:1.6;">
                        Account email:
                        <strong>{ownerEmail}</strong>
                    </p>

                    <p style="font-size:15px; line-height:1.6;">
                        To activate your account and set your password, click the button below:
                    </p>

                    <div style="margin:30px 0; text-align:center;">
                        <a
                            href="{activationUrl}"
                            style="display:inline-block; padding:12px 24px; background-color:#2563eb; color:#ffffff; text-decoration:none; border-radius:8px; font-weight:bold;"
                        >
                            Activate Account
                        </a>
                    </div>

                    <p style="font-size:13px; line-height:1.6; color:#6b7280;">
                        If the button does not work, copy and paste the following link into your browser:
                    </p>

                    <p style="font-size:13px; line-height:1.6; word-break:break-all;">
                        <a href="{activationUrl}" style="color:#2563eb;">
                            {activationUrl}
                        </a>
                    </p>

                    <p style="font-size:13px; line-height:1.6; color:#6b7280;">
                        This link is required to activate your account and set your password.
                    </p>

                    <p style="font-size:15px; line-height:1.6; margin-top:30px;">
                        Best regards,<br/>
                        <strong>ILS Egypt</strong>
                    </p>

                </div>
            </body>
            </html>
            """;
    }
}
