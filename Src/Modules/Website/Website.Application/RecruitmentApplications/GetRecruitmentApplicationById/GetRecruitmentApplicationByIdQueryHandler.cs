using BuildingBlocks.Application;
using MediatR;
using Website.Application.Abstractions.Repositories;
using Website.Domain.Entities;

namespace Website.Application.RecruitmentApplications.GetRecruitmentApplicationById;

public sealed class GetRecruitmentApplicationByIdQueryHandler(
    IRecruitmentApplicationRepository recruitmentApplicationRepository)
    : IRequestHandler<
        GetRecruitmentApplicationByIdQuery,
        Result<RecruitmentApplication>>
{
    public async Task<Result<RecruitmentApplication>> Handle(
        GetRecruitmentApplicationByIdQuery query,
        CancellationToken ct)
    {
        var application =
            await recruitmentApplicationRepository.GetByIdAsync(
                query.Id,
                ct);

        if (application is null)
            return Result.Failure<RecruitmentApplication>(
                "Recruitment application was not found.");

        return Result.Success(application);
    }
}