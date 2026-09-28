using Content.Application.Content.GetContentChildren;
using MediatR;

namespace Api.Modules.Content.GetContentChildren;

public static class GetContentChildrenEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/content/{parentId:guid}/children",
            async (
                Guid parentId,
                ISender sender,
                CancellationToken ct) =>
            {
                var query = new GetContentChildrenQuery(parentId);

                var result = await sender.Send(query, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            });
    }
}