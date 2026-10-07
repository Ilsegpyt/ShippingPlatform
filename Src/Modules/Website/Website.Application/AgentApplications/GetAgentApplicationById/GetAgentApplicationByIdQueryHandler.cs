using BuildingBlocks.Application;
using MediatR;
using Website.Application.Abstractions.Repositories;
using Website.Domain.Entities;

namespace Website.Application.AgentApplications.GetAgentApplicationById;

public sealed class GetAgentApplicationByIdQueryHandler(
    IAgentApplicationRepository agentApplicationRepository)
    : IRequestHandler<
        GetAgentApplicationByIdQuery,
        Result<AgentApplication>>
{
    public async Task<Result<AgentApplication>> Handle(
        GetAgentApplicationByIdQuery query,
        CancellationToken ct)
    {
        var application =
            await agentApplicationRepository.GetByIdAsync(
                query.Id,
                ct);

        if (application is null)
            return Result.Failure<AgentApplication>(
                "Agent application was not found.");

        return Result.Success(application);
    }
}