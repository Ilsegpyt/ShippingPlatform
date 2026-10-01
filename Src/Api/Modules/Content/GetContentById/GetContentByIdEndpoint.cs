using Content.Application.Content.GetContentById;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;

namespace Api.Modules.Content.GetContentById;

public static class GetContentByIdEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/content/{id:guid}", async (
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            var query = new GetContentByIdQuery(id);

            var result = await sender.Send(query, ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound(result.Error);
        })
            .RequirePermission(PermissionCatalog.ContentView);
    }
}