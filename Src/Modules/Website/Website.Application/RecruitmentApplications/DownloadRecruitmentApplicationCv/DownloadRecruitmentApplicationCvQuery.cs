using BuildingBlocks.Application;
using MediatR;
using Website.Domain.Entities;

namespace Website.Application.RecruitmentApplications.DownloadRecruitmentApplicationCv;

public sealed record DownloadRecruitmentApplicationCvQuery(
    int Id)
    : IRequest<Result<RecruitmentApplication>>;