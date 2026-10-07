using Api.Modules.Website.AgentApplications.CreateAgentApplication;
using Api.Modules.Website.AgentApplications.GetAgentApplication;
using Api.Modules.Website.AgentApplications.GetAgentById;
using Api.Modules.Website.RecruitmentApplications.CreateRecruitmentApplication;
using Api.Modules.Website.RecruitmentApplications.GetRecruitmentApplicationById;
using Api.Modules.Website.RecruitmentApplications.GetRecruitmentApplications;

namespace Api.Modules.Website;

public static class WebsiteEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        CreateAgentApplicationEndpoint.Map(app);
        GetAgentApplicationsEndpoint.Map(app);
        GetAgentApplicationByIdEndpoint.Map(app);


        CreateRecruitmentApplicationEndpoint.Map(app);
        GetRecruitmentApplicationsEndpoint.Map(app);
        GetRecruitmentApplicationByIdEndpoint.Map(app);
    }
}