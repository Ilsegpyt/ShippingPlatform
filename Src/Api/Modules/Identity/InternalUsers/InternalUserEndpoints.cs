using Identity.Api.InternalUsers;
using Identity.Application.InternalUsers.CreateInternalUser;
using Identity.Application.InternalUsers.DeleteInternalUser;
using Identity.Application.InternalUsers.UpdateInternalUserEmail;
using Identity.Application.InternalUsers.UpdateInternalUserProfile;
using Identity.Domain;
using Identity.Infrastructure.Authorization;
using MediatR;

namespace Api.Modules.Identity.InternalUsers;

public static class InternalUserEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("/api/internal-users")
            .WithTags("InternalUsers");

        MapCreate(users);
        MapUpdateProfile(users);
        MapUpdateEmail(users);
        MapDelete(users);
    }

    private static void MapCreate(IEndpointRouteBuilder users)
    {
        users.MapPost("/", async (
            CreateInternalUserCommand command,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.UsersCreate);
    }

    private static void MapUpdateProfile(IEndpointRouteBuilder users)
    {
        users.MapPut("/{id:guid}", async (
            Guid id,
            UpdateInternalUserProfileRequestBody body,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new UpdateInternalUserProfileCommand(
                    id,
                    body.Name,
                    body.Phone),
                ct);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.UsersEdit);
    }

    private static void MapUpdateEmail(IEndpointRouteBuilder users)
    {
        users.MapPut("/{id:guid}/email", async (
            Guid id,
            UpdateInternalUserEmailRequestBody body,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new UpdateInternalUserEmailCommand(
                    id,
                    body.Email),
                ct);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.UsersEdit);
    }

    private static void MapDelete(IEndpointRouteBuilder users)
    {
        users.MapDelete("/{id:guid}", async (
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new DeleteInternalUserCommand(id),
                ct);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.UsersDelete);
    }
}