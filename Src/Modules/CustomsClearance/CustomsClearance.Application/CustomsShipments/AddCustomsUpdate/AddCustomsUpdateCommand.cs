using BuildingBlocks.Application;
using MediatR;

namespace CustomsClearance.Application.CustomsShipments.AddCustomsUpdate;

public sealed record AddCustomsUpdateCommand(
    Guid ShipmentId,
    string UpdateText)
    : IRequest<Result>;
