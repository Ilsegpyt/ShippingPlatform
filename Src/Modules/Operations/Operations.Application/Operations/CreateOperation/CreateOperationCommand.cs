using BuildingBlocks.Application;
using MediatR;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.CreateOperation;

public sealed record CreateOperationCommand(
    // Common
    Guid ClientId,
    Guid ShippingLineId,
    OperationType OperationType,

    // Import
    string? MBLNumber,
    string? Destination,
    Guid? PODId,
    DateOnly? FreeTimeTill,
    DateOnly? RequiredOffloadingDate,
    TimeOnly? RequiredOffloadingTime,

    // Export
    ClearanceType? ClearanceType,
    string? BookingNumber,
    Guid? POLId,
    Guid? POWId,
    DateOnly? CutOffDate

) : IRequest<Result<Guid>>;