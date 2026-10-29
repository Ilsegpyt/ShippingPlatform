using BuildingBlocks.Application;
using MediatR;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.UpdateExport;

public sealed record UpdateExportCommand(
    Guid OperationId,
    string? ShipmentNumber,
    string? CertificateNumber,
    string? Customs,
    IReadOnlyCollection<UpdateExportContainerItem> Containers,
    Guid? POLId = null,
    Guid? PODId = null,
    Guid? POWId = null,
    DateTime? InvoicesReceivedDate = null,
    bool ClearPOD = false,
    bool ClearPOW = false
) : IRequest<Result<bool>>;

public sealed record UpdateExportContainerItem(
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
