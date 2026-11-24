using BuildingBlocks.Application;
using MediatR;

namespace Transporters.Application.ClientExceptions.AddClientException;

public sealed record AddClientExceptionCommand(
    Guid TransporterId,
    Guid PortCoverageId,
    Guid CustomerId,
    string Destination,
    string EquipmentType,
    decimal AgreedPrice,
    DateTime ValidFromUtc,
    DateTime ValidToUtc,
    string? Reason)
    : IRequest<Result>;
