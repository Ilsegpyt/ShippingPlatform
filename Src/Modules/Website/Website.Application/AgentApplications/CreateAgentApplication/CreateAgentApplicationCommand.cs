using BuildingBlocks.Application;
using MediatR;

namespace Website.Application.AgentApplications.CreateAgentApplication;

public sealed record CreateAgentApplicationCommand(
    string FirstName,
    string LastName,
    string Country,
    string City,
    string Address,
    string Email,
    string CountryCode,
    string Phone,
    string MainIndustry,
    string Message)
    : IRequest<Result>;