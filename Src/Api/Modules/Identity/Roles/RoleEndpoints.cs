using Identity.Application.Roles.ActivateRole;
using Identity.Application.Roles.CreateRole;
using Identity.Application.Roles.DeactivateRole;
using Identity.Application.Roles.GetRolePermissions;
using Identity.Application.Roles.GetRoles;
using Identity.Application.Roles.GrantPermissionToRole;
using Identity.Application.Roles.RevokePermissionFromRole;
using Identity.Domain.ValueObjects;
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
        MapGetAll(roles);
        MapGetPermissions(roles);
        MapRevokePermission(roles);
        MapDeactivate(roles);
        MapActivate(roles);
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

    private static void MapGetAll(IEndpointRouteBuilder roles)
    {
        roles.MapGet("/", async (
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new GetRolesQuery(),
                ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.RolesManage);
    }

    private static void MapGetPermissions(
        IEndpointRouteBuilder roles)
    {
        roles.MapGet("/{id:guid}/permissions", async (
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new GetRolePermissionsQuery(id),
                ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.RolesManage);
    }

    private static void MapRevokePermission(
        IEndpointRouteBuilder roles)
    {
        roles.MapDelete(
            "/{id:guid}/permissions/{permissionKey}",
            async (
                Guid id,
                string permissionKey,
                ISender sender,
                CancellationToken ct) =>
            {
                var command = new RevokePermissionFromRoleCommand(
                    id,
                    permissionKey);

                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.BadRequest(result.Error);
            })
            .RequirePermission(PermissionCatalog.RolesManage);
    }

    private static void MapDeactivate(
    IEndpointRouteBuilder roles)
    {
        roles.MapDelete(
            "/{id:guid}",
            async (
                Guid id,
                ISender sender,
                CancellationToken ct) =>
            {
                var command = new DeactivateRoleCommand(id);

                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.BadRequest(result.Error);
            })
            .RequirePermission(PermissionCatalog.RolesManage);
    }
    private static void MapActivate(
    IEndpointRouteBuilder roles)
    {
        roles.MapPost(
            "/{id:guid}/activate",
            async (
                Guid id,
                ISender sender,
                CancellationToken ct) =>
            {
                var command = new ActivateRoleCommand(id);

                var result = await sender.Send(
                    command,
                    ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.BadRequest(result.Error);
            })
            .RequirePermission(PermissionCatalog.RolesManage);
    }
}