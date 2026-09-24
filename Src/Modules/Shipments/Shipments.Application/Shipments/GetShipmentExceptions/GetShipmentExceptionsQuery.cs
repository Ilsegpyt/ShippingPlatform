using BuildingBlocks.Application;
using MediatR;

namespace Shipments.Application.Shipments.GetShipmentExceptions;

public sealed record GetShipmentExceptionsQuery(
    Guid UserId,
    string? TokenType,
    string? OrganizationId)
    : IRequest<Result<GetShipmentExceptionsResponse>>;

public sealed record GetShipmentExceptionsResponse(
    int Total,
    int MissingDocuments,
    int Cancelled);