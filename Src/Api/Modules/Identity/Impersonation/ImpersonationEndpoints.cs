
using Identity.Application.Impersonation.EndImpersonation;
using Identity.Application.Impersonation.ImpersonateCustomer;
using Identity.Domain;
using Identity.Infrastructure.Authorization;
using MediatR;
using System.Security.Claims;

namespace Api.Modules.Identity.Impersonation;

public static class ImpersonationEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/api/identity/impersonation/customer",
                async (
                    ImpersonateCustomerRequest request,
                    HttpContext httpContext,
                    ClaimsPrincipal user,
                    ISender sender,
                    CancellationToken ct) =>
                {
                    var impersonatorUserId = user.GetUserId();

                    var command = new ImpersonateCustomerCommand(
                        impersonatorUserId,
                        request.CustomerUserId,
                        httpContext.Connection.RemoteIpAddress?.ToString(),
                        httpContext.Request.Headers.UserAgent.ToString(),
                        request.Reason);

                    var result = await sender.Send(command, ct);

                    if (result.IsFailure)
                        return Results.BadRequest(result.Error);

                    return Results.Ok(result.Value);
                })
            .RequirePermission(
                PermissionCatalog.CustomersImpersonate);

        app.MapPost(
                "/api/identity/impersonation/end",
                async (
                    EndImpersonationRequest request,
                    ClaimsPrincipal user,
                    ISender sender,
                    CancellationToken ct) =>
                {
                    var impersonatorUserId = user.GetUserId();

                    var command = new EndImpersonationCommand(
                        request.AuditLogId,
                        impersonatorUserId);

                    var result = await sender.Send(command, ct);

                    if (result.IsFailure)
                        return Results.BadRequest(result.Error);

                    return Results.Ok();
                })
            .RequireAuthorization();
    }

    public sealed record ImpersonateCustomerRequest(
        Guid CustomerUserId,
        string? Reason);

    public sealed record EndImpersonationRequest(
        Guid AuditLogId);
}

