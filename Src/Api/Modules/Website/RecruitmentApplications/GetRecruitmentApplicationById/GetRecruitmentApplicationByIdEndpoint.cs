using MediatR;
using Website.Application.RecruitmentApplications.GetRecruitmentApplicationById;

namespace Api.Modules.Website.RecruitmentApplications.GetRecruitmentApplicationById;

public static class GetRecruitmentApplicationByIdEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/website/recruitment-applications/{id:int}",
            async (
                int id,
                ISender sender,
                CancellationToken ct) =>
            {
                var query = new GetRecruitmentApplicationByIdQuery(id);

                var result = await sender.Send(query, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.NotFound(result.Error);
            });
    }
}