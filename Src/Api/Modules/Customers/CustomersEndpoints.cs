using BuildingBlocks.Application;
using Customers.Application.Customers.ActivateCustomer;
using Customers.Application.Customers.DeleteCustomer;
using Customers.Application.Customers.RegisterCustomer;
using Customers.Application.Customers.SuspendCustomer;
using Customers.Application.Customers.UpdateCustomerEmail;
using Customers.Application.Customers.UpdateCustomerProfile;
using Customers.Application.Queries.GetCustomerById;
using Customers.Application.Queries.ListAllCustomers;
using Customers.Application.Queries.ListCustomers;
using Customers.Application.Schedules.SearchCustomerMultiSchedules;
using Customers.Application.Schedules.SearchCustomerSchedules;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Modules.Customers;

public static class CustomersEndpoints
{
    public static void MapCustomersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/customers");

        MapGetAll(group);
        MapGetAllIncludingDeleted(group);
        MapGetById(group);
        MapUpdate(group);
        MapSuspend(group);
        MapActivate(group);
        MapRegister(group);
        MapUpdateEmail(group);
        MapSearchSchedules(group);
        MapMultiSearchSchedules(group);
        MapDelete(group);
    }

    private static void MapGetAll(IEndpointRouteBuilder customers)
    {
        customers.MapGet("/", async (
            [AsParameters] PaginationRequest pagination,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new ListCustomersQuery(pagination),
                ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        });
    }

    private static void MapGetAllIncludingDeleted(IEndpointRouteBuilder group)
    {
        group.MapGet("/all", async (
            [FromQuery] bool deletedOnly,
            [AsParameters] PaginationRequest pagination,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new ListAllCustomersQuery(deletedOnly, pagination),
                ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.CustomersView);
    }

    private static void MapGetById(IEndpointRouteBuilder group)
    {
        group.MapGet("/{id:guid}", async (
            Guid id,
            ISender sender) =>
        {
            var result = await sender.Send(
                new GetCustomerByIdQuery(id));

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound(result.Error);
        })
        .RequirePermission(PermissionCatalog.CustomersView);
    }

    private static void MapUpdate(IEndpointRouteBuilder group)
    {
        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateCustomerProfileRequest body,
            ISender sender) =>
        {
            var result = await sender.Send(
                new UpdateCustomerProfileCommand(
                    id,
                    body.OwnerName,
                    body.CompanyName,
                    body.OwnerPhone,
                    body.Industry));

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.CustomersEdit);
    }
    private static void MapUpdateEmail(IEndpointRouteBuilder group)
    {
        group.MapPut("/{id:guid}/email", async (
            Guid id,
            UpdateCustomerEmailRequest body,
            ISender sender) =>
        {
            var result = await sender.Send(
                new UpdateCustomerEmail(
                    id,
                    body.Email));

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.CustomersEdit);
    }
    private static void MapSuspend(IEndpointRouteBuilder group)
    {
        group.MapPost("/{id:guid}/suspend", async (
            Guid id,
            ISender sender) =>
        {
            var result = await sender.Send(
                new SuspendCustomerCommand(id));

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.CustomersSuspend);
    }

    private static void MapActivate(IEndpointRouteBuilder group)
    {
        group.MapPost("/{id:guid}/activate", async (
            Guid id,
            ISender sender) =>
        {
            var result = await sender.Send(
                new ActivateCustomerCommand(id));

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.CustomersSuspend);
    }

    private static void MapRegister(IEndpointRouteBuilder group)
    {
        group.MapPost("/", async (
            RegisterCustomerRequest body,
            ISender sender) =>
        {
            var result = await sender.Send(
                new RegisterCustomerCommand(
                    body.OwnerName,
                    body.CompanyName,
                    body.OwnerPhone,
                    body.OwnerEmail,
                    body.Industry));

            return result.IsSuccess
                ? Results.Created(
                    $"/api/customers/{result.Value.CustomerId}",
                    result.Value)
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.CustomersCreate);
    }
    private static void MapMultiSearchSchedules(IEndpointRouteBuilder group)
    {
        group.MapPost("/schedules/multi-search", async (
            [FromBody] SearchCustomerMultiSchedulesRequest request,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken ct) =>
        {
            var customerId = user.GetOrganizationId();

            var result = await sender.Send(
                new SearchCustomerMultiSchedulesQuery(
                    request.Routes,
                    customerId),
                ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        }).RequirePermission(PermissionCatalog.SchedulesSearch);
    }
    private static void MapSearchSchedules(IEndpointRouteBuilder group)
    {
        group.MapGet("/schedules/search", async (
            [FromQuery] string origin,
            [FromQuery] string destination,
            [FromQuery] DateOnly departureDate,
            [FromQuery] string containerSize,
            ClaimsPrincipal user,
            ISender sender) =>
        {
            var customerId = user.GetOrganizationId();

            var result = await sender.Send(
                new SearchCustomerSchedulesQuery(
                    origin,
                    destination,
                    departureDate,
                    containerSize,
                    customerId));

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        }).RequirePermission(PermissionCatalog.SchedulesSearch);
    }
    private static void MapDelete(IEndpointRouteBuilder group)
    {
        group.MapDelete("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken ct) =>
        {
            var deletedByUserId = user.GetUserId();

            var result = await sender.Send(
                new DeleteCustomerCommand(id, deletedByUserId),
                ct);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        })
        .RequirePermission(PermissionCatalog.CustomersDelete);
    }


}

