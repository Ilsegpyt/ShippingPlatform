using CustomsClearance.Application.CustomsShipments.CreateCustomsShipment;
using CustomsClearance.Domain.Enums;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;

namespace Api.Modules.CustomsClearance.CustomsShipments.CreateCustomsShipment;

public static class CreateCustomsShipmentEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/api/customs-clearance/shipments",
            async (
                CreateCustomsShipmentRequest request,
                ISender sender,
                CancellationToken ct) =>
            {
                var command = new CreateCustomsShipmentCommand(
                    request.ReportType,
                    request.ClientId,
                    request.OceanImportDetails,
                    request.OceanExportDetails,
                    request.DomesticImportDetails);

                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            }).RequirePermission(PermissionCatalog.CustomsShipmentsCreate);
    }
}

public sealed record CreateCustomsShipmentRequest(
    ReportType ReportType,
    Guid ClientId,
    CreateOceanImportDetails? OceanImportDetails,
    CreateOceanExportDetails? OceanExportDetails,
    CreateDomesticImportDetails? DomesticImportDetails);
