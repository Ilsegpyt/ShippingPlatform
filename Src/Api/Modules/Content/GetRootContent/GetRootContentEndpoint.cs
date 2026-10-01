using Content.Application.Content.GetRootContent;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;

namespace Api.Modules.Content.GetRootContent;

public static class GetRootContentEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/content/root",
            async (
                ISender sender,
                CancellationToken ct) =>
            {
                var query = new GetRootContentQuery();

                var result = await sender.Send(query, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            })
            .RequirePermission(PermissionCatalog.ContentView);
    }
}