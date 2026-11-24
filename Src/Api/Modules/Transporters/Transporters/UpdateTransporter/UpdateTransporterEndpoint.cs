using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Transporters.Application.Transporters.UpdateTransporter;

namespace Api.Modules.Transporters.Transporters.UpdateTransporter;

public static class UpdateTransporterEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut(
            "/api/transporters/{id:guid}",
            async (
                Guid id,
                UpdateTransporterRequest request,
                ISender sender,
                CancellationToken ct) =>
            {
                var command = new UpdateTransporterCommand(
                    id,
                    request.Name,
                    request.ContactPerson,
                    request.Phone,
                    request.Email,
                    request.IsActive);

                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Ok()
                    : Results.BadRequest(result.Error);
            }).RequirePermission(PermissionCatalog.TransportersEdit);
    }
}

public sealed record UpdateTransporterRequest(
    string Name,
    string? ContactPerson,
    string? Phone,
    string? Email,
    bool IsActive);
