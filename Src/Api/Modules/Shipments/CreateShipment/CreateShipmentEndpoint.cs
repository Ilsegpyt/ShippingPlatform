using Identity.Domain;
using Identity.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shipments.Application.Shipments.CreateShipment;
using System.Security.Claims;

namespace Api.Modules.Shipments.CreateShipment;

public static class CreateShipmentEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/shipments", async (
            [FromForm] CreateShipmentRequest request,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken ct) =>
        {
            var customerId = user.GetOrganizationId();

            var command = new CreateShipmentCommand(
                customerId,
                request.ScheduleId,
                request.Quantity,
                request.DeclarationFiles
                    .Select(file => new DeclarationFileInput(
                        file.FileName,
                        file.OpenReadStream()))
                    .ToList());

            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        }).DisableAntiforgery().RequirePermission(PermissionCatalog.ShipmentsBook);
    }
}