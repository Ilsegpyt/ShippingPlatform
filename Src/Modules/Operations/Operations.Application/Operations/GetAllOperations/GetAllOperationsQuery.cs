using BuildingBlocks.Application;
using MediatR;

namespace Operations.Application.Operations.GetAllOperations;

public sealed record GetAllOperationsQuery
    : IRequest<Result<IReadOnlyList<OperationDto>>>;

public sealed record OperationDto(
    Guid Id,
    DateTime CreatedAtUtc,
    string CreatedByName,
    Guid ClientId,
    string? ClientName,
    Guid ShippingLineId,
    string ShippingLineName,
    string OperationType,
    string Status,
    string Source,
    string? ShipmentNumber,
    string? CertificateNumber,
    DateTime? InvoicesReceivedDate,
    DateTime? DeliveredAtUtc,
    string? DeliveredByUserId,
    DateTime? ReopenedAtUtc,
    string? ReopenedByUserId,
    string? ReopenReason,
    int NumberOfContainers,
    ImportDetailsDto? ImportDetails,
    ExportDetailsDto? ExportDetails,
    IReadOnlyList<OperationContainerDto> Containers);

public sealed record ImportDetailsDto(
    Guid Id,
    string MBLNumber,
    Guid PODId,
    string PODName,
    string? PODCode,
    DateOnly RequiredOffloadingDate,
    TimeOnly RequiredOffloadingTime);

public sealed record ExportDetailsDto(
    Guid Id,
    string ClearanceType,
    string BookingNumber,
    Guid POLId,
    string POLName,
    string? POLCode,
    Guid? POWId,
    string? POWName,
    string? POWCode,
    DateOnly? CutOffDate,
    string? Customs);

public sealed record OperationContainerDto(
    Guid Id,
    Guid OperationId,
    short Ordinal,
    string? ContainerNumber,
    string? ContainerType,
    string? Destination,
    DateOnly? FreeTimeTill,
    DateOnly? RequiredLoadingDate,
    TimeOnly? RequiredLoadingTime,
    DateOnly? ActualArrivalDate,
    TimeOnly? ActualArrivalTime,
    DateOnly? GateOutDate,
    TimeOnly? GateOutTime,
    string? MissingDocs,
    DateOnly? MissingDocsReceivedDate,
    DateTime? CheckOutFromThePort,
    Guid? TransporterId,
    string? TransporterName,
    Guid? DriverId,
    string? DriverNumber,
    string? DriverName,
    string? DriverMobileNumber,
    Guid? VehicleId,
    string? VehicleCarNumber,
    IReadOnlyList<ContainerUpdateDto> Updates);

public sealed record ContainerUpdateDto(
    Guid Id,
    Guid OperationContainerId,
    string Details,
    DateTime CreatedAtUtc,
    string CreatedByUserId,
    string CreatedByName);
