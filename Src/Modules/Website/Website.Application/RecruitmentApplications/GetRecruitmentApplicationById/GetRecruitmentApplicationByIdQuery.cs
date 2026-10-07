using BuildingBlocks.Application;
using MediatR;
using Website.Domain.Entities;

namespace Website.Application.RecruitmentApplications.GetRecruitmentApplicationById;

public sealed record GetRecruitmentApplicationByIdQuery(
    int Id)
    : IRequest<Result<RecruitmentApplication>>;