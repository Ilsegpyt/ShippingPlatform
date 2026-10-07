using MediatR;
using Website.Application.Branches.GetBranches;

namespace Api.Modules.Website.Branches.GetBranches;

public static class GetBranchesEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/website/branches",
            async (
                ISender sender,
                CancellationToken ct) =>
            {
                var query = new GetBranchesQuery();

                var result = await sender.Send(query, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            });
    }
}