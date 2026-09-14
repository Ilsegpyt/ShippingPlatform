using System.Security.Claims;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Shipments.Application.Shipments.GetDeclarationFilesByShipmentId;

namespace Api.Modules.Shipments.GetDeclarationFilesByShipmentId;

public static class GetDeclarationFilesByShipmentIdEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/shipments/{shipmentId:guid}/declaration-files",
            async (
                Guid shipmentId,
                ISender sender,
                ClaimsPrincipal user,
                CancellationToken ct) =>
            {
                var customerId = user.GetOrganizationId();

                var result = await sender.Send(
                    new GetDeclarationFilesByShipmentIdQuery(
                        shipmentId,
                        customerId),
                    ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.NotFound(result.Error);
            })
        .RequirePermission(PermissionCatalog.ShipmentsView);
    }
}