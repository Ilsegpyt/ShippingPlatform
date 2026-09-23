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

            var tokenType = user.FindFirst("token_type")?.Value;

            var organizationId = user.FindFirst("org_id")?.Value;

            var notifications =
                await notificationQueries.GetByUserIdAsync(
                    userId,
                    tokenType,
                    organizationId,
                    ct);

            return Results.Ok(notifications);
        })
        .RequirePermission(PermissionCatalog.NotificationsView);

        app.MapPatch("/api/notifications/{notificationId:guid}/read", async (
            Guid notificationId,
            ClaimsPrincipal user,
            NotificationQueries notificationQueries,
            CancellationToken ct) =>
        {
            var userId = user.GetUserId();

            var tokenType = user.FindFirst("token_type")?.Value;

            var organizationId = user.FindFirst("org_id")?.Value;

            var marked =
                await notificationQueries.MarkAsReadAsync(
                    notificationId,
                    userId,
                    tokenType,
                    organizationId,
                    ct);

            return marked
                ? Results.NoContent()
                : Results.NotFound();
        })
        .RequirePermission(PermissionCatalog.NotificationsView);
    }
}