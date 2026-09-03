using BuildingBlocks.Application;
using MediatR;

namespace Shipments.Application.Shipments.CreateShipment;

public sealed record CreateShipmentCommand(
    Guid CustomerId,
    Guid ScheduleId,
    int Quantity,
    IReadOnlyCollection<DeclarationFileInput> DeclarationFiles)
    : IRequest<Result<CreateShipmentResponse>>;

public sealed record CreateShipmentResponse(
    Guid ShipmentId,
    string ShipmentRef,
    decimal Total);