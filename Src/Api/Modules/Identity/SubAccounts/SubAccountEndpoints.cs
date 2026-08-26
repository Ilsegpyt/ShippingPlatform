using Identity.Application.SubAccounts.ActivateSubAccount;
using Identity.Application.SubAccounts.CreateSubAccount;
using Identity.Application.SubAccounts.DeleteSubAccount;
using Identity.Application.SubAccounts.GetSubAccounts;
using Identity.Application.SubAccounts.GrantSubAccountPermission;
using Identity.Application.SubAccounts.ResetSubAccountPassword;
using Identity.Application.SubAccounts.RevokeSubAccountPermission;
using Identity.Application.SubAccounts.SetCustomScope;
using Identity.Application.SubAccounts.SetFullScope;
using Identity.Application.SubAccounts.UpdateSubAccountEmail;
using Identity.Application.SubAccounts.UpdateSubAccountProfile;
using Identity.Application.SubAccounts.UpdateSubAccountScope;
using Identity.Domain;
using Identity.Infrastructure.Authorization;
using MediatR;
using System.Security.Claims;

namespace Api.Modules.Identity.SubAccounts;

public static class SubAccountEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var subAccounts = app.MapGroup("/api/subaccounts")
            .WithTags("SubAccounts");

        MapCreate(subAccounts);
        MapGetAll(subAccounts);
        MapUpdate(subAccounts);
        MapDelete(subAccounts);
        MapActivation(subAccounts);
        MapPasswordReset(subAccounts);
        MapPermissions(subAccounts);
        MapScopes(subAccounts);
        MapUpdateEmail(subAccounts);
    }

    private static void MapCreate(IEndpointRouteBuilder subAccounts)
    {
        subAccounts.MapPost("/", async (
            CreateSubAccountRequestBody body,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken ct) =>
        {
            var organizationId = user.GetOrganizationId();

            var command = new CreateSubAccountCommand(
                organizationId,
                body.Name,
                body.Email,
                body.GrantFullScope,
                body.Scopes?
                    .Select(s => new ScopeInput(
                        s.Category,
                        s.Service,
                        s.Type))
                    .ToList() ?? []);

            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.SubAccountsCreate);
    }

    private static void MapGetAll(IEndpointRouteBuilder subAccounts)
    {
        subAccounts.MapGet("/", async (
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken ct) =>
        {
            var organizationId = user.GetOrganizationId();

            var result = await sender.Send(
                new GetSubAccountsQuery(organizationId),
                ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.SubAccountsView);
    }

    private static void MapUpdate(IEndpointRouteBuilder subAccounts)
    {
        subAccounts.MapPut("/{id:guid}", async (
            Guid id,
            UpdateSubAccountProfileRequestBody body,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken ct) =>
        {
            var organizationId = user.GetOrganizationId();

            var result = await sender.Send(
                new UpdateSubAccountProfileCommand(
                    organizationId,
                    id,
                    body.Name),
                ct);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.SubAccountsEdit);
    }
    private static void MapUpdateEmail(IEndpointRouteBuilder subAccounts)
    {
        subAccounts.MapPut("/{id:guid}/email", async (
            Guid id,
            UpdateSubAccountEmailRequestBody body,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new UpdateSubAccountEmailCommand(
                    id,
                    body.Email),
                ct);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.SubAccountsEdit);
    }
    private static void MapDelete(IEndpointRouteBuilder subAccounts)
    {
        subAccounts.MapDelete("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken ct) =>
        {
            var organizationId = user.GetOrganizationId();

            var result = await sender.Send(
                new DeleteSubAccountCommand(
                    organizationId,
                    id),
                ct);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.SubAccountsDelete);
    }

    private static void MapActivation(IEndpointRouteBuilder subAccounts)
    {
        subAccounts.MapPost("/{id:guid}/activate", async (
            Guid id,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken ct) =>
        {
            var organizationId = user.GetOrganizationId();

            var result = await sender.Send(
                new ActivateSubAccountCommand(
                    organizationId,
                    id),
                ct);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.SubAccountsSuspend);

        subAccounts.MapPost("/{id:guid}/deactivate", async (
            Guid id,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken ct) =>
        {
            var organizationId = user.GetOrganizationId();

            var result = await sender.Send(
                new DeactivateSubAccountCommand(
                    organizationId,
                    id),
                ct);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.SubAccountsSuspend);
    }

    private static void MapPasswordReset(IEndpointRouteBuilder subAccounts)
    {
        subAccounts.MapPost("/{id:guid}/reset-password", async (
            Guid id,
            ResetSubAccountPasswordRequestBody body,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken ct) =>
        {
            var organizationId = user.GetOrganizationId();

            var result = await sender.Send(
                new ResetSubAccountPasswordCommand(
                    organizationId,
                    id,
                    body.NewPassword),
                ct);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.SubAccountsEdit);
    }

    private static void MapPermissions(IEndpointRouteBuilder subAccounts)
    {
        subAccounts.MapPost("/{id:guid}/permissions", async (
            Guid id,
            GrantSubAccountPermissionRequestBody body,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken ct) =>
        {
            var organizationId = user.GetOrganizationId();

            var result = await sender.Send(
                new GrantSubAccountPermissionCommand(
                    organizationId,
                    id,
                    body.PermissionKey),
                ct);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.SubAccountsEdit);

        subAccounts.MapDelete(
            "/{id:guid}/permissions/{permissionKey}",
            async (
                Guid id,
                string permissionKey,
                ClaimsPrincipal user,
                ISender sender,
                CancellationToken ct) =>
            {
                var organizationId = user.GetOrganizationId();

                var result = await sender.Send(
                    new RevokeSubAccountPermissionCommand(
                        organizationId,
                        id,
                        permissionKey),
                    ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.BadRequest(result.Error);
            })
            .RequirePermission(PermissionCatalog.SubAccountsEdit);
    }

    private static void MapScopes(IEndpointRouteBuilder subAccounts)
    {
        subAccounts.MapPost("/{id:guid}/scopes", async (
            Guid id,
            AddScopeRequestBody body,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken ct) =>
        {
            var organizationId = user.GetOrganizationId();

            var result = await sender.Send(
                new AddScopeCommand(
                    organizationId,
                    id,
                    body.Category,
                    body.Service,
                    body.Type),
                ct);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.SubAccountsEdit);

        subAccounts.MapDelete("/{id:guid}/scopes", async (
            Guid id,
            ScopeCategory category,
            ScopeService service,
            ScopeShipmentType type,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken ct) =>
        {
            var organizationId = user.GetOrganizationId();

            var result = await sender.Send(
                new RemoveScopeCommand(
                    organizationId,
                    id,
                    category,
                    service,
                    type),
                ct);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.SubAccountsEdit);

        subAccounts.MapPost("/{id:guid}/full-scope", async (
            Guid id,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken ct) =>
        {
            var organizationId = user.GetOrganizationId();

            var result = await sender.Send(
                new SetFullScopeCommand(
                    organizationId,
                    id),
                ct);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.SubAccountsEdit);

        subAccounts.MapPost("/{id:guid}/custom-scope", async (
            Guid id,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken ct) =>
        {
            var organizationId = user.GetOrganizationId();

            var result = await sender.Send(
                new SetCustomScopeCommand(
                    organizationId,
                    id),
                ct);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.SubAccountsEdit);
    }
}