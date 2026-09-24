namespace Notifications.Application.Templates.Emails;

public static class SubAccountCreatedEmailTemplate
{
    public static string Build(
        string name,
        string email,
        string activationUrl)
    {
        return $"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8" />
                <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                <title>Welcome to ILS</title>
            </head>

            <body style="
                margin: 0;
                padding: 0;
                background-color: #f4f6f8;
                font-family: Arial, Helvetica, sans-serif;
                color: #333333;
            ">
                <table
                    width="100%"
                    cellpadding="0"
                    cellspacing="0"
                    border="0"
                    style="padding: 40px 20px;"
                >
                    <tr>
                        <td align="center">

                            <table
                                width="600"
                                cellpadding="0"
                                cellspacing="0"
                                border="0"
                                style="
                                    max-width: 600px;
                                    width: 100%;
                                    background-color: #ffffff;
                                    border-radius: 8px;
                                    overflow: hidden;
                                "
                            >

                                <tr>
                                    <td style="
                                        padding: 30px 40px;
                                        background-color: #1f2937;
                                        color: #ffffff;
                                    ">
                                        <h1 style="
                                            margin: 0;
                                            font-size: 24px;
                                            font-weight: 600;
                                        ">
                                            Welcome to ILS
                                        </h1>
                                    </td>
                                </tr>

                                <tr>
                                    <td style="padding: 40px;">

                                        <p style="
                                            margin: 0 0 20px;
                                            font-size: 16px;
                                        ">
                                            Hello <strong>{name}</strong>,
                                        </p>

                                        <p style="
                                            margin: 0 0 20px;
                                            font-size: 15px;
                                            line-height: 1.6;
                                        ">
                                            Your ILS sub-account has been created successfully.
                                        </p>

                                        <p style="
                                            margin: 0 0 10px;
                                            font-size: 15px;
                                            line-height: 1.6;
                                        ">
                                            Account email:
                                        </p>

                                        <p style="
                                            margin: 0 0 30px;
                                            font-size: 15px;
                                            font-weight: 600;
                                        ">
                                            {email}
                                        </p>

                                        <p style="
                                            margin: 0 0 25px;
                                            font-size: 15px;
                                            line-height: 1.6;
                                        ">
                                            To activate your account and create your password,
                                            click the button below:
                                        </p>

                                        <table
                                            cellpadding="0"
                                            cellspacing="0"
                                            border="0"
                                            style="margin: 0 auto 30px;"
                                        >
                                            <tr>
                                                <td
                                                    align="center"
                                                    style="
                                                        border-radius: 6px;
                                                        background-color: #2563eb;
                                                    "
                                                >
                                                    <a
                                                        href="{activationUrl}"
                                                        style="
                                                            display: inline-block;
                                                            padding: 14px 28px;
                                                            font-size: 15px;
                                                            font-weight: 600;
                                                            color: #ffffff;
                                                            text-decoration: none;
                                                            border-radius: 6px;
                                                        "
                                                    >
                                                        Activate My Account
                                                    </a>
                                                </td>
                                            </tr>
                                        </table>

                                        <p style="
                                            margin: 0 0 15px;
                                            font-size: 13px;
                                            line-height: 1.6;
                                            color: #666666;
                                        ">
                                            If the button above does not work, copy and paste
                                            the following link into your browser:
                                        </p>

                                        <p style="
                                            margin: 0 0 30px;
                                            font-size: 12px;
                                            line-height: 1.6;
                                            word-break: break-all;
                                        ">
                                            <a
                                                href="{activationUrl}"
                                                style="color: #2563eb;"
                                            >
                                                {activationUrl}
                                            </a>
                                        </p>

                                        <p style="
                                            margin: 0;
                                            font-size: 13px;
                                            line-height: 1.6;
                                            color: #666666;
                                        ">
                                            For security reasons, this activation link should
                                            only be used by the account owner.
                                        </p>

                                    </td>
                                </tr>

                                <tr>
                                    <td style="
                                        padding: 25px 40px;
                                        border-top: 1px solid #eeeeee;
                                        background-color: #fafafa;
                                    ">
                                        <p style="
                                            margin: 0;
                                            font-size: 12px;
                                            line-height: 1.5;
                                            color: #777777;
                                        ">
                                            If you did not expect to receive this email,
                                            please contact ILS Egypt support.
                                        </p>

                                        <p style="
                                            margin: 15px 0 0;
                                            font-size: 12px;
                                            color: #777777;
                                        ">
                                            Best regards,<br />
                                            <strong>ILS Egypt</strong>
                                        </p>
                                    </td>
                                </tr>

                            </table>

                        </td>
                    </tr>
                </table>
            </body>
            </html>
            """;
    }
}