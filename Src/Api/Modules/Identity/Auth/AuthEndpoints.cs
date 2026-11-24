
using Identity.Application.Abstractions;
using Identity.Application.Auth.ActivateAccount;
using Identity.Application.Auth.ChangePassword;
using Identity.Application.Auth.ForgotPassword;
using Identity.Application.Auth.Login;
using Identity.Application.Auth.RefreshToken;
using Identity.Application.Auth.ResetPassword;
using Identity.Infrastructure.Authorization;
using MediatR;

namespace Api.Modules.Identity.Auth;

public static class AuthEndpoints
{
    private const string RefreshTokenCookieName = "refreshToken";

    private static bool IsSecureRequest(HttpContext httpContext)
    {
        return httpContext.Request.IsHttps ||
               string.Equals(httpContext.Request.Headers["X-Forwarded-Proto"], "https", StringComparison.OrdinalIgnoreCase);
    }

    private static void SetRefreshTokenCookie(HttpContext httpContext, string refreshToken, DateTime expiresAtUtc)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = IsSecureRequest(httpContext),
            SameSite = SameSiteMode.Lax,
            Expires = expiresAtUtc,
            Path = "/"
        };

        httpContext.Response.Cookies.Append(RefreshTokenCookieName, refreshToken, cookieOptions);
    }

    private static void DeleteRefreshTokenCookie(HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete(RefreshTokenCookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = IsSecureRequest(httpContext),
            SameSite = SameSiteMode.Lax,
            Path = "/"
        });
    }

    public static void Map(IEndpointRouteBuilder app)
    {
        var auth = app.MapGroup("/api/auth")
            .WithTags("Auth");

        // Login
        auth.MapPost(
            "/login",
            async (
                LoginCommand command,
                HttpContext httpContext,
                ISender sender,
                CancellationToken ct) =>
            {
                var result = await sender.Send(command, ct);

                if (result.IsFailure)
                    return Results.Unauthorized();

                SetRefreshTokenCookie(httpContext, result.Value.RefreshToken, DateTime.UtcNow.AddDays(14));

                return Results.Ok(result.Value);
            })
            .RequireRateLimiting("LoginPolicy");

        // Forgot Password
        auth.MapPost(
            "/forgot-password",
            async (
                ForgotPasswordCommand command,
                ISender sender,
                CancellationToken ct) =>
            {
                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Ok()
                    : Results.BadRequest(result.Error);
            });

        // Refresh Token
        auth.MapPost(
            "/refresh",
            async (
                RefreshTokenRequest? request = null,
                HttpContext httpContext = null!,
                ISender sender = null!,
                CancellationToken ct = default) =>
            {
                var token = request?.RefreshToken;
                if (string.IsNullOrWhiteSpace(token))
                {
                    token = httpContext.Request.Cookies[RefreshTokenCookieName];
                }

                if (string.IsNullOrWhiteSpace(token))
                    return Results.Unauthorized();

                var command = new RefreshTokenCommand(token);
                var result = await sender.Send(command, ct);

                if (result.IsFailure)
                {
                    DeleteRefreshTokenCookie(httpContext);
                    return Results.Unauthorized();
                }

                SetRefreshTokenCookie(httpContext, result.Value.RefreshToken, DateTime.UtcNow.AddDays(14));

                return Results.Ok(result.Value);
            });

        // Logout
        auth.MapPost(
            "/logout",
            async (
                HttpContext httpContext,
                ITokenService tokenService,
                IIdentityUnitOfWork unitOfWork,
                CancellationToken ct) =>
            {
                var token = httpContext.Request.Cookies[RefreshTokenCookieName];
                if (!string.IsNullOrWhiteSpace(token))
                {
                    await tokenService.RevokeAsync(token, ct);
                    await unitOfWork.SaveChangesAsync(ct);
                }

                DeleteRefreshTokenCookie(httpContext);
                return Results.Ok();
            });

        // Activate Account
        auth.MapPost(
            "/activate",
            async (
                ActivateAccountCommand command,
                ISender sender,
                CancellationToken ct) =>
            {
                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            });

        // Change Password
        auth.MapPost(
            "/change-password",
            async (
                ChangePasswordCommand command,
                HttpContext httpContext,
                ISender sender,
                CancellationToken ct) =>
            {
                var userId =
                    ClaimsPrincipalExtensions.GetUserId(
                        httpContext.User);

                var result = await sender.Send(
                    command with { UserId = userId },
                    ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            })
            .RequireAuthorization();

        // Reset Password
        auth.MapPost(
            "/reset-password",
            async (
                ResetPasswordCommand command,
                ISender sender,
                CancellationToken ct) =>
            {
                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Ok()
                    : Results.BadRequest(result.Error);
            });
    }

    public sealed record RefreshTokenRequest(string? RefreshToken);
}
