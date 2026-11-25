using CustomsClearance.Application.CustomsShipments.UpdateCustomsShipment;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;

namespace Api.Modules.CustomsClearance.CustomsShipments.UpdateCustomsShipment;

public static class UpdateCustomsShipmentEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut(
            "/api/customs-clearance/shipments/{id:guid}",
            async (
                Guid id,
                UpdateCustomsShipmentRequest request,
                ISender sender,
                CancellationToken ct) =>
            {
                var command = new UpdateCustomsShipmentCommand(
                    id,
                    request.ShipmentNumber,
                    request.CertificateNumber,
                    request.OceanImportDetails,
                    request.OceanExportDetails,
                    request.DomesticImportDetails);

                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Ok()
                    : Results.BadRequest(result.Error);
            }).RequirePermission(PermissionCatalog.CustomsShipmentsEdit);
    }
}

public sealed record UpdateCustomsShipmentRequest(
    string? ShipmentNumber,
    string? CertificateNumber,
    UpdateOceanImportDetails? OceanImportDetails,
    UpdateOceanExportDetails? OceanExportDetails,
    UpdateDomesticImportDetails? DomesticImportDetails);
