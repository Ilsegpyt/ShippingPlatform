using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using Notifications.Application.Services;
using System.Security.Claims;

namespace Api.Modules.Notifications;

public static class NotificationEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/notifications", async (
            ClaimsPrincipal user,
            NotificationQueries notificationQueries,
            CancellationToken ct) =>
        {
            var userId = user.GetUserId();

            var notifications = await notificationQueries.GetByUserIdAsync(
                userId,
                ct);

            return Results.Ok(notifications);
        }).RequirePermission(PermissionCatalog.NotificationsView);

        app.MapPatch("/api/notifications/{notificationId:guid}/read", async (
            Guid notificationId,
            ClaimsPrincipal user,
            NotificationQueries notificationQueries,
            CancellationToken ct) =>
        {
            var userId = user.GetUserId();

            var marked = await notificationQueries.MarkAsReadAsync(
                notificationId,
                userId,
                ct);

            return marked
                ? Results.NoContent()
                : Results.NotFound();
        }).RequirePermission(PermissionCatalog.NotificationsView);
    }
}