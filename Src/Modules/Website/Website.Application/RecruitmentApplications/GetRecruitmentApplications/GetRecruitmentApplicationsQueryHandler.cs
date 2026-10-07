using BuildingBlocks.Application;
using MediatR;
using Website.Application.Abstractions.Repositories;

namespace Website.Application.RecruitmentApplications.GetRecruitmentApplications;

public sealed class GetRecruitmentApplicationsQueryHandler(
    IRecruitmentApplicationRepository recruitmentApplicationRepository)
    : IRequestHandler<
        GetRecruitmentApplicationsQuery,
        Result<PagedResult<Domain.Entities.RecruitmentApplication>>>
{
    public async Task<Result<PagedResult<Domain.Entities.RecruitmentApplication>>> Handle(
        GetRecruitmentApplicationsQuery query,
        CancellationToken ct)
    {
        var pageNumber = query.Pagination.PageNumber;
        var pageSize = query.Pagination.PageSize;

        var (items, totalCount) =
            await recruitmentApplicationRepository.GetPagedAsync(
                pageNumber,
                pageSize,
                ct);

        var result =
            new PagedResult<Domain.Entities.RecruitmentApplication>(
                items,
                totalCount,
                pageNumber,
                pageSize);

        return Result.Success(result);
    }
}