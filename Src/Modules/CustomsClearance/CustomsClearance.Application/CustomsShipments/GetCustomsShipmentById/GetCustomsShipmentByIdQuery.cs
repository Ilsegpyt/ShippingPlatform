using BuildingBlocks.Application;
using CustomsClearance.Domain.Entities;
using MediatR;

namespace CustomsClearance.Application.CustomsShipments.GetCustomsShipmentById;

public sealed record GetCustomsShipmentByIdQuery(
    Guid Id)
    : IRequest<Result<CustomsShipment>>;
