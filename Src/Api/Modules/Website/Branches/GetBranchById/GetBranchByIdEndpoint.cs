using MediatR;
using Website.Application.Branches.GetBranchById;

namespace Api.Modules.Website.Branches.GetBranchById;

public static class GetBranchByIdEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/website/branches/{id:int}",
            async (
                int id,
                ISender sender,
                CancellationToken ct) =>
            {
                var query = new GetBranchByIdQuery(id);

                var result = await sender.Send(query, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.NotFound(result.Error);
            });
    }
}