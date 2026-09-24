using BuildingBlocks.Application;
using MediatR;

namespace Shipments.Application.Shipments.GetShipmentExceptions;

public sealed record GetShipmentExceptionsQuery
    : IRequest<Result<GetShipmentExceptionsResponse>>;

public sealed record GetShipmentExceptionsResponse(
    int Total,
    int MissingDocuments,
    int Cancelled);