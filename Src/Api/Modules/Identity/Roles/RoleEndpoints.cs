using Identity.Application.Roles.CreateRole;
using Identity.Application.Roles.GrantPermissionToRole;
using Identity.Domain;
using Identity.Infrastructure.Authorization;
using MediatR;

namespace Api.Modules.Identity.Roles;

public static class RoleEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var roles = app.MapGroup("/api/roles")
            .WithTags("Roles");

        MapCreate(roles);
        MapPermissions(roles);
    }

    private static void MapCreate(IEndpointRouteBuilder roles)
    {
        roles.MapPost("/", async (
            CreateRoleCommand command,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.Ok(new { RoleId = result.Value })
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.RolesManage);
    }

    private static void MapPermissions(IEndpointRouteBuilder roles)
    {
        roles.MapPost("/{id:guid}/permissions", async (
            Guid id,
            GrantPermissionRequestBody body,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new GrantPermissionToRoleCommand(
                id,
                body.PermissionKey);

            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.RolesManage);
    }
}
