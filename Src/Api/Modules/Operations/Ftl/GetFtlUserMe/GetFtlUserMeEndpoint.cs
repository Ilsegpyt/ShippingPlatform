using BuildingBlocks.Application.Abstractions;

namespace Api.Modules.Operations.GetFtlUserMe;

public static class GetFtlUserMeEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/ftl/user/me", async (ICurrentUser currentUser, CancellationToken ct) =>
        {
            var userName = await currentUser.GetUserNameAsync(ct);
            var userId = currentUser.UserId != Guid.Empty ? currentUser.UserId.ToString() : "u-1";
            return Results.Ok(new { id = userId, displayName = userName });
        }).RequireAuthorization();

        app.MapGet("/getCurrentUser", async (ICurrentUser currentUser, CancellationToken ct) =>
        {
            var userName = await currentUser.GetUserNameAsync(ct);
            var userId = currentUser.UserId != Guid.Empty ? currentUser.UserId.ToString() : "u-1";
            return Results.Ok(new { id = userId, displayName = userName });
        }).RequireAuthorization();

        app.MapGet("/api/v1/ftl/permissions", () => Results.Ok(new
        {
            reopen_shipments = true,
            delete_shipments = true
        })).RequireAuthorization();

        app.MapGet("/loadPermissions", () => Results.Ok(new
        {
            reopen_shipments = true,
            delete_shipments = true
        })).RequireAuthorization();
    }
}

