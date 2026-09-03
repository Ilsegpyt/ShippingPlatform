using BuildingBlocks.Application;
using MediatR;

namespace Shipments.Application.Shipments.DeleteShipments;

public sealed record DeleteShipmentsCommand(
    IReadOnlyCollection<Guid> ShipmentIds)
    : IRequest<Result<DeleteShipmentsResponse>>;

public sealed record DeleteShipmentsResponse(
    int DeletedCount);