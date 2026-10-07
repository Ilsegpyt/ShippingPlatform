using BuildingBlocks.Application;
using MediatR;
using Website.Domain.Entities;

namespace Website.Application.AgentApplications.GetAgentApplicationById;

public sealed record GetAgentApplicationByIdQuery(
    int Id)
    : IRequest<Result<AgentApplication>>;