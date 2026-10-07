using BuildingBlocks.Application;
using MediatR;
using Website.Application.Abstractions;
using Website.Application.Abstractions.Repositories;
using Website.Domain.Entities;

namespace Website.Application.AgentApplications.CreateAgentApplication;

public sealed class CreateAgentApplicationCommandHandler(
    IAgentApplicationRepository agentApplicationRepository,
    IWebsiteUnitOfWork websiteUnitOfWork)
    : IRequestHandler<CreateAgentApplicationCommand, Result>
{
    public async Task<Result> Handle(
        CreateAgentApplicationCommand cmd,
        CancellationToken ct)
    {
        var application = new AgentApplication
        {
            FirstName = cmd.FirstName,
            LastName = cmd.LastName,
            Country = cmd.Country,
            City = cmd.City,
            Address = cmd.Address,
            Email = cmd.Email,
            CountryCode = cmd.CountryCode,
            Phone = cmd.Phone,
            MainIndustry = cmd.MainIndustry,
            Message = cmd.Message,
            CreatedAtUtc = DateTime.UtcNow
        };

        agentApplicationRepository.Add(application);

        await websiteUnitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}