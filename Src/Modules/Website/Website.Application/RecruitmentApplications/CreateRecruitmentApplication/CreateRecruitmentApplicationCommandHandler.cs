using BuildingBlocks.Application;
using MediatR;
using Website.Application.Abstractions;
using Website.Application.Abstractions.Repositories;
using Website.Domain.Entities;

namespace Website.Application.RecruitmentApplications.CreateRecruitmentApplication;

public sealed class CreateRecruitmentApplicationCommandHandler(
    IRecruitmentApplicationRepository recruitmentApplicationRepository,
    IWebsiteUnitOfWork websiteUnitOfWork)
    : IRequestHandler<CreateRecruitmentApplicationCommand, Result>
{
    public async Task<Result> Handle(
        CreateRecruitmentApplicationCommand cmd,
        CancellationToken ct)
    {
        var application = new RecruitmentApplication
        {
            Department = cmd.Department,
            CvFileName = cmd.CvFileName,
            CvFilePath = cmd.CvFilePath,
            Message = cmd.Message,
            CreatedAtUtc = DateTime.UtcNow
        };

        recruitmentApplicationRepository.Add(application);

        await websiteUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}