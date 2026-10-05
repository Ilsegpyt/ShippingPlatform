using BuildingBlocks.Application;
using MediatR;
using Operations.Domain.Enums;

namespace Operations.Application.Operations.CreateOperation;

public sealed record CreateOperationCommand(
    Guid ClientId,
    Guid ShippingLineId,
    OperationType OperationType,
    string? ShipmentNumber,
    string? CertificateNumber,
    DateTime? InvoicesReceivedDate
) : IRequest<Result<Guid>>;