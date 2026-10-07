using Api.Modules.Website.AgentApplications.CreateAgentApplication;
using Api.Modules.Website.AgentApplications.GetAgentApplication;
using Api.Modules.Website.AgentApplications.GetAgentById;
using Api.Modules.Website.Branches.CreateBranch;
using Api.Modules.Website.Branches.DeleteBranch;
using Api.Modules.Website.Branches.GetBranchById;
using Api.Modules.Website.Branches.GetBranches;
using Api.Modules.Website.Branches.RestoreBranch;
using Api.Modules.Website.Branches.UpdateBranch;
using Api.Modules.Website.ContactInquiries.CreateContactInquiry;
using Api.Modules.Website.ContactInquiries.GetContactInquiries;
using Api.Modules.Website.ContactInquiries.GetContactInquiryById;
using Api.Modules.Website.QuoteRequests.CreateQuoteRequest;
using Api.Modules.Website.QuoteRequests.GetQuoteRequestById;
using Api.Modules.Website.QuoteRequests.GetQuoteRequests;
using Api.Modules.Website.RecruitmentApplications.CreateRecruitmentApplication;
using Api.Modules.Website.RecruitmentApplications.DownloadRecruitmentApplicationCv;
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


        CreateContactInquiryEndpoint.Map(app);
        GetContactInquiriesEndpoint.Map(app);
        GetContactInquiryByIdEndpoint.Map(app);



        CreateQuoteRequestEndpoint.Map(app);
        GetQuoteRequestsEndpoint.Map(app);
        GetQuoteRequestByIdEndpoint.Map(app);


        GetBranchesEndpoint.Map(app);
        GetBranchByIdEndpoint.Map(app);
        CreateBranchEndpoint.Map(app);
        UpdateBranchEndpoint.Map(app);
        DeleteBranchEndpoint.Map(app);
        RestoreBranchEndpoint.Map(app);
        DownloadRecruitmentApplicationCvEndpoint.Map(app);
    }
}