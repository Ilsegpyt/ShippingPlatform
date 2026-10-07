using BuildingBlocks.Application;
using MediatR;
using Website.Application.AgentApplications.GetAgentApplications;

namespace Api.Modules.Website.AgentApplications.GetAgentApplication;

public static class GetAgentApplicationsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/website/agent-applications",
            async (
                [AsParameters] PaginationRequest pagination,
                ISender sender,
                CancellationToken ct) =>
            {
                var query = new GetAgentApplicationsQuery(
                    pagination);

                var result = await sender.Send(query, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            });
    }
}