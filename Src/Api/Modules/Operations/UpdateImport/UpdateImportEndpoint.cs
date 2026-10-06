using MediatR;
using Operations.Application.Operations.UpdateImport;
using Operations.Domain.Enums;

namespace Api.Modules.Operations.UpdateImport;

public static class UpdateImportEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut(
            "/api/operations/{operationId:guid}/import",
            async (
                Guid operationId,
                UpdateImportRequest request,
                ISender sender,
                CancellationToken ct) =>
            {
                var command = new UpdateImportCommand(
                    operationId,
                    request.ShipmentNumber,
                    request.CertificateNumber,
                    request.InvoicesReceivedDate,
                    request.Status,
                    request.Containers
                        .Select(x => new UpdateImportContainerItem(
                            x.ContainerId,
                            x.ActualArrivalDate,
                            x.ActualArrivalTime,
                            x.GateOutDate,
                            x.GateOutTime,
                            x.ContainerType,
                            x.ContainerNumber,
                            x.TransporterId,
                            x.VehicleId,
                            x.DriverId,
                            x.CheckOutFromThePort,
                            x.MissingDocs,
                            x.MissingDocsReceivedDate))
                        .ToList());

                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            });
    }
}

public sealed record UpdateImportRequest(
    string? ShipmentNumber,
    string? CertificateNumber,
    DateTime? InvoicesReceivedDate,
    OperationStatus? Status,
    IReadOnlyCollection<UpdateImportContainerRequest> Containers);

public sealed record UpdateImportContainerRequest(
    Guid ContainerId,

    DateOnly? ActualArrivalDate,
    TimeOnly? ActualArrivalTime,

    DateOnly? GateOutDate,
    TimeOnly? GateOutTime,

    ContainerType? ContainerType,
    string? ContainerNumber,

    Guid? TransporterId,
    Guid? VehicleId,
    Guid? DriverId,

    DateTime? CheckOutFromThePort,

    string? MissingDocs,
    DateOnly? MissingDocsReceivedDate);