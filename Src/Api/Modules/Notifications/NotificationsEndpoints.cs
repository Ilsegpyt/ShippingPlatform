namespace Api.Modules.Notifications;

public static class NotificationsEndpoints
{
    public static IEndpointRouteBuilder MapNotificationsEndpoints(
        this IEndpointRouteBuilder app)
    {
        NotificationEndpoints.Map(app);

        return app;
    }
}