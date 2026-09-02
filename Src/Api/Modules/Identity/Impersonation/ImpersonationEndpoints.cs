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
               .RequirePermission(PermissionCatalog.ImpersonateCustomer);
           
    }
}

public sealed record ImpersonateCustomerRequest(
    Guid CustomerUserId,
    string? Reason);