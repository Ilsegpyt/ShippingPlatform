using Identity.Application.AccountManagerAssignments.AssignAccountManager;
using Identity.Application.AccountManagerAssignments.GetAssignments;
using Identity.Application.AccountManagerAssignments.GetUnassignedCustomers;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;

namespace Api.Modules.Identity.AccountManagerAssignments;

public static class AccountManagerAssignmentEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var assignments = app.MapGroup("/api/account-manager-assignments")
            .WithTags("AccountManagerAssignments");

        MapGetAll(assignments);
        MapGetUnassigned(assignments);
        MapAssign(assignments);
    }

    private static void MapGetAll(IEndpointRouteBuilder assignments)
    {
        assignments.MapGet("/", async (
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new GetAccountManagerAssignmentsQuery(),
                ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.UsersView);
    }
    private static void MapGetUnassigned(
    IEndpointRouteBuilder assignments)
    {
        assignments.MapGet("/unassigned", async (
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new GetUnassignedCustomersQuery(),
                ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.UsersView);
    }
    private static void MapAssign(
    IEndpointRouteBuilder assignments)
    {
        assignments.MapPost("/", async (
            AssignAccountManagerCommand command,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                command,
                ct);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.UsersView);
    }
}