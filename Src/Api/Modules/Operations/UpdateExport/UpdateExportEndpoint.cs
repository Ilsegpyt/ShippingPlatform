using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.UpdateExport;
using Operations.Domain.Enums;

namespace Api.Modules.Operations.UpdateExport;

public static class UpdateExportEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/operations/{operationId:guid}/export", async (
            Guid operationId,
            UpdateExportRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateExportCommand(
                operationId,
                request.ShipmentNumber,
                request.CertificateNumber,
                request.Customs,
                (request.Containers ?? Array.Empty<UpdateExportContainerRequest>())
                    .Select(x => new UpdateExportContainerItem(
                        x.ContainerId,
                        x.ContainerType,
                        x.ContainerNumber,
                        x.RequiredLoadingDate,
                        x.RequiredLoadingTime,
                        x.Destination,
                        x.ActualArrivalDate,
                        x.ActualArrivalTime,
                        x.GateOutDate,
                        x.GateOutTime,
                        x.TransporterId,
                        x.VehicleId,
                        x.DriverId,
                        x.MissingDocs,
                        x.MissingDocsReceivedDate,
                        x.CarNumber))
                    .ToArray(),
                request.POLId,
                request.PODId,
                request.POWId,
                request.InvoicesReceivedDate,
                request.ClearPOD,
                request.ClearPOW
            );

            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.Ok(new { Success = result.Value })
                : Results.BadRequest(new { Error = result.Error });
        }).RequirePermission(PermissionCatalog.OperationsEdit);
    }
}

public sealed record UpdateExportRequest(
    string? ShipmentNumber,
    string? CertificateNumber,
    string? Customs,
    IReadOnlyCollection<UpdateExportContainerRequest> Containers,
    Guid? POLId = null,
    Guid? PODId = null,
    Guid? POWId = null,
    DateTime? InvoicesReceivedDate = null,
    bool ClearPOD = false,
    bool ClearPOW = false
);

public sealed record UpdateExportContainerRequest(
    Guid ContainerId,
    ContainerType? ContainerType,
    string? ContainerNumber,
    DateOnly? RequiredLoadingDate,
    TimeOnly? RequiredLoadingTime,
    string? Destination,
    DateOnly? ActualArrivalDate = null,
    TimeOnly? ActualArrivalTime = null,
    DateOnly? GateOutDate = null,
    TimeOnly? GateOutTime = null,
    Guid? TransporterId = null,
    Guid? VehicleId = null,
    Guid? DriverId = null,
    string? MissingDocs = null,
    DateOnly? MissingDocsReceivedDate = null,
    string? CarNumber = null
);
