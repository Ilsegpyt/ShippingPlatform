
using Identity.Application.Impersonation.EndImpersonation;
using Identity.Application.Impersonation.ImpersonateCustomer;
using Identity.Domain.ValueObjects;
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

                    var isHttps = httpContext.Request.IsHttps ||
                                  string.Equals(httpContext.Request.Headers["X-Forwarded-Proto"], "https", StringComparison.OrdinalIgnoreCase);

                    httpContext.Response.Cookies.Append("refreshToken", result.Value.RefreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = isHttps,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTime.UtcNow.AddDays(14),
                        Path = "/"
                    });

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

