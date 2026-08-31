using BuildingBlocks.Application;
using BuildingBlocks.Domain;
using Identity.Application.AccountManagerAssignments.AssignAccountManager;
using Identity.Application.AccountManagerAssignments.ChangeAccountManager;
using Identity.Application.AccountManagerAssignments.RemoveAccountManager;
using Identity.Domain;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Identity.AccountManagerAssignments;

public static class AccountManagerAssignmentEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/account-manager-assignments");

        MapAssign(group);
        MapChange(group);
        MapRemove(group);
    }

    private static void MapAssign(IEndpointRouteBuilder group)
    {
        group.MapPost("/", async (
            AssignAccountManagerCommand command,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.Ok()
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(
            PermissionCatalog.CustomersAssignAccountManager);
    }

    private static void MapChange(IEndpointRouteBuilder group)
    {
        group.MapPut("/", async (
            ChangeAccountManagerCommand command,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.Ok()
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(
            PermissionCatalog.CustomersAssignAccountManager);
    }
    private static void MapRemove(IEndpointRouteBuilder group)
    { 
        group.MapDelete("/", async (
       [FromBody] RemoveAccountManagerCommand command,
        ISender sender, 
        CancellationToken ct) =>
    { 
        var result = await sender.Send(command, ct);
        return result.IsSuccess
        ? Results.Ok()
        : Results.BadRequest(result.Error);
    }).RequirePermission(PermissionCatalog.CustomersAssignAccountManager);
    }
}

