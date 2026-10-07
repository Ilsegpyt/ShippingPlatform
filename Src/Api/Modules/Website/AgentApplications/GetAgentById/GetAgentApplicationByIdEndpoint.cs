using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Website.Application.AgentApplications.GetAgentApplicationById;

namespace Api.Modules.Website.AgentApplications.GetAgentById;

public static class GetAgentApplicationByIdEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/website/agent-applications/{id:int}",
            async (
                int id,
                ISender sender,
                CancellationToken ct) =>
            {
                var query = new GetAgentApplicationByIdQuery(id);

                var result = await sender.Send(query, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.NotFound(result.Error);
            }).RequirePermission(PermissionCatalog.AgentApplicationsView);
    }
}