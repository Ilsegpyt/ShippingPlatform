using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Transporters.Application.Transporters.CreateTransporter;

namespace Api.Modules.Transporters.Transporters.CreateTransporter;

public static class CreateTransporterEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/api/transporters",
            async (
                CreateTransporterRequest request,
                ISender sender,
                CancellationToken ct) =>
            {
                var command = new CreateTransporterCommand(
                    request.Name,
                    request.ContactPerson,
                    request.Phone,
                    request.Email);

                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            }).RequirePermission(PermissionCatalog.TransportersCreate);
    }
}

public sealed record CreateTransporterRequest(
    string Name,
    string? ContactPerson,
    string? Phone,
    string? Email);
