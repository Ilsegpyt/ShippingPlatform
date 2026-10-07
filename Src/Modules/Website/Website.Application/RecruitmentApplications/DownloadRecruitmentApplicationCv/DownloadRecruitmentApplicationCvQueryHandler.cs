using BuildingBlocks.Application;
using MediatR;
using Website.Application.Abstractions.Repositories;
using Website.Domain.Entities;

namespace Website.Application.RecruitmentApplications.DownloadRecruitmentApplicationCv;

public sealed class DownloadRecruitmentApplicationCvQueryHandler(
    IRecruitmentApplicationRepository recruitmentApplicationRepository)
    : IRequestHandler<
        DownloadRecruitmentApplicationCvQuery,
        Result<RecruitmentApplication>>
{
    public async Task<Result<RecruitmentApplication>> Handle(
        DownloadRecruitmentApplicationCvQuery query,
        CancellationToken ct)
    {
        var application =
            await recruitmentApplicationRepository.GetByIdAsync(
                query.Id,
                ct);

        if (application is null)
        {
            return Result.Failure<RecruitmentApplication>(
                "Recruitment application was not found.");
        }

        if (string.IsNullOrWhiteSpace(application.CvFilePath))
        {
            return Result.Failure<RecruitmentApplication>(
                "CV file is not available.");
        }

        return Result.Success(application);
    }
}