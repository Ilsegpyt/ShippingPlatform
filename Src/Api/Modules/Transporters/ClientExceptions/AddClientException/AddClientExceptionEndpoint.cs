using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Transporters.Application.ClientExceptions.AddClientException;

namespace Api.Modules.Transporters.ClientExceptions.AddClientException;

public static class AddClientExceptionEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/api/transporters/{id:guid}/ports/{portCoverageId:guid}/client-exceptions",
            async (
                Guid id,
                Guid portCoverageId,
                AddClientExceptionRequest request,
                ISender sender,
                CancellationToken ct) =>
            {
                var command = new AddClientExceptionCommand(
                    id,
                    portCoverageId,
                    request.CustomerId,
                    request.Destination,
                    request.EquipmentType,
                    request.AgreedPrice,
                    request.ValidFromUtc,
                    request.ValidToUtc,
                    request.Reason);

                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Ok()
                    : Results.BadRequest(result.Error);
            }).RequirePermission(PermissionCatalog.TransportersEdit);
    }
}

public sealed record AddClientExceptionRequest(
    Guid CustomerId,
    string Destination,
    string EquipmentType,
    decimal AgreedPrice,
    DateTime ValidFromUtc,
    DateTime ValidToUtc,
    string? Reason);
