using BuildingBlocks.Application;
using MediatR;
using Shipments.Domain.Shipments;

namespace Shipments.Application.Shipments.UpdateShipment;

public sealed record UpdateShipmentCommand(
    Guid ShipmentId,
    ShipmentStatus Status,
    string? MBL,
    string? HBL,
    string? MAWB,
    string? BookingConfirmationNumber)
    : IRequest<Result<UpdateShipmentResponse>>;

public sealed record UpdateShipmentResponse(
    Guid ShipmentId,
    string ShipmentRef);