using BuildingBlocks.Application;
using MediatR;
using Website.Domain.Entities;

namespace Website.Application.RecruitmentApplications.GetRecruitmentApplications;

public sealed record GetRecruitmentApplicationsQuery(
    PaginationRequest Pagination)
    : IRequest<Result<PagedResult<RecruitmentApplication>>>;