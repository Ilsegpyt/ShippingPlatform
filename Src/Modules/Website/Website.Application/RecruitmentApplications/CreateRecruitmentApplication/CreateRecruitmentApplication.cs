using BuildingBlocks.Application;
using MediatR;

namespace Website.Application.RecruitmentApplications.CreateRecruitmentApplication;

public sealed record CreateRecruitmentApplicationCommand(
    string Department,
    string CvFileName,
    string CvFilePath,
    string Message)
    : IRequest<Result>;