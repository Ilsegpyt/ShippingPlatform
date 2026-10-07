using BuildingBlocks.Application;
using MediatR;
using Website.Application.RecruitmentApplications.GetRecruitmentApplications;

namespace Api.Modules.Website.RecruitmentApplications.GetRecruitmentApplications;

public static class GetRecruitmentApplicationsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/website/recruitment-applications",
            async (
                [AsParameters] PaginationRequest pagination,
                ISender sender,
                CancellationToken ct) =>
            {
                var query = new GetRecruitmentApplicationsQuery(
                    pagination);

                var result = await sender.Send(query, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            });
    }
}