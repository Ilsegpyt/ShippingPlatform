using BuildingBlocks.Application;
using MediatR;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.UpdateImport;

public sealed record UpdateImportCommand(
    Guid OperationId,

    // Operation
    string? ShipmentNumber,
    string? CertificateNumber,
    DateTime? InvoicesReceivedDate,
    OperationStatus? Status,

    // Containers
    IReadOnlyCollection<UpdateImportContainerItem> Containers

) : IRequest<Result<bool>>;


public sealed record UpdateImportContainerItem(
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
    DateOnly? MissingDocsReceivedDate
);