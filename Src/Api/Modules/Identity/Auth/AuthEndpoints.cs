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
    public static void Map(IEndpointRouteBuilder app)
    {
        var auth = app.MapGroup("/api/auth")
            .WithTags("Auth");

        auth.MapPost("/login", async (LoginCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Unauthorized();
        });
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
        auth.MapPost("/refresh", async (RefreshTokenCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Unauthorized();
        });

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
        auth.MapPost(
    "/change-password",
    async (
        ChangePasswordCommand command,
        HttpContext httpContext,
        ISender sender,
        CancellationToken ct) =>
    {
        var userId = ClaimsPrincipalExtensions.GetUserId(httpContext.User);

        var result = await sender.Send(
            command with { UserId = userId },
            ct);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Error);
    })
    .RequireAuthorization();

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


}
