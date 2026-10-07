using BuildingBlocks.Application;
using MediatR;
using Website.Application.Abstractions.Repositories;

namespace Website.Application.AgentApplications.GetAgentApplications;

public sealed class GetAgentApplicationsQueryHandler(
    IAgentApplicationRepository agentApplicationRepository)
    : IRequestHandler<
        GetAgentApplicationsQuery,
        Result<PagedResult<Domain.Entities.AgentApplication>>>
{
    public async Task<Result<PagedResult<Domain.Entities.AgentApplication>>> Handle(
        GetAgentApplicationsQuery query,
        CancellationToken ct)
    {
        var pageNumber = query.Pagination.PageNumber;
        var pageSize = query.Pagination.PageSize;

        var (items, totalCount) =
            await agentApplicationRepository.GetPagedAsync(
                pageNumber,
                pageSize,
                ct);

        var result = new PagedResult<Domain.Entities.AgentApplication>(
            items,
            totalCount,
            pageNumber,
            pageSize);

        return Result.Success(result);
    }
}