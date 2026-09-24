namespace Notifications.Application.Options;

public sealed class NotificationOptions
{
    public const string SectionName = "Notifications";

    public string FrontendBaseUrl { get; set; } = string.Empty;
}