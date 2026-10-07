using Api.Modules.Website.AgentApplications.CreateAgentApplication;
using Api.Modules.Website.AgentApplications.GetAgentApplication;
using Api.Modules.Website.AgentApplications.GetAgentById;

namespace Api.Modules.Website;

public static class WebsiteEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        CreateAgentApplicationEndpoint.Map(app);
        GetAgentApplicationsEndpoint.Map(app);
        GetAgentApplicationByIdEndpoint.Map(app);
    }
}