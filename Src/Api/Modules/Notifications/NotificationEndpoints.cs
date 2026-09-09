using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using Notifications.Application.Services;
using System.Security.Claims;

namespace Api.Modules.Notifications.Notifications;

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
    }
}