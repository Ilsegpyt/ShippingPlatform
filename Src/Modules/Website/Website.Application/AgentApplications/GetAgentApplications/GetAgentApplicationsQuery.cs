using BuildingBlocks.Application;
using MediatR;
using Website.Domain.Entities;

namespace Website.Application.AgentApplications.GetAgentApplications;

public sealed record GetAgentApplicationsQuery(
    PaginationRequest Pagination)
    : IRequest<Result<PagedResult<AgentApplication>>>;