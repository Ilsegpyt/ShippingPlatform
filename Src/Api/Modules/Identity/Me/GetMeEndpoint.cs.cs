using Identity.Application.Me.GetMe;
using Identity.Infrastructure.Authorization;
using MediatR;
using System.Security.Claims;

namespace Api.Modules.Identity.Me;

public static class GetMeEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/me", async (
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken ct) =>
        {
            var userId = user.GetUserId();

            var result = await sender.Send(
                new GetMeQuery(userId),
                ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        });
    }
}