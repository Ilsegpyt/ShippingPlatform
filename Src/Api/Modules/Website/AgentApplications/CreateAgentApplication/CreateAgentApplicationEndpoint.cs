using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Website.Application.AgentApplications.CreateAgentApplication;

namespace Api.Modules.Website.AgentApplications.CreateAgentApplication;

public static class CreateAgentApplicationEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/api/website/agent-applications",
            async (
                CreateAgentApplicationRequest request,
                ISender sender,
                CancellationToken ct) =>
            {
                var command = new CreateAgentApplicationCommand(
                    request.FirstName,
                    request.LastName,
                    request.Country,
                    request.City,
                    request.Address,
                    request.Email,
                    request.CountryCode,
                    request.Phone,
                    request.MainIndustry,
                    request.Message);

                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Ok()
                    : Results.BadRequest(result.Error);
            }).RequirePermission(PermissionCatalog.AgentApplicationsCreate);
    }
}

public sealed record CreateAgentApplicationRequest(
    string FirstName,
    string LastName,
    string Country,
    string City,
    string Address,
    string Email,
    string CountryCode,
    string Phone,
    string MainIndustry,
    string Message);