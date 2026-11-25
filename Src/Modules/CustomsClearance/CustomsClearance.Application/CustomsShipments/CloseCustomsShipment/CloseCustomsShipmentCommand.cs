using BuildingBlocks.Application;
using MediatR;

namespace CustomsClearance.Application.CustomsShipments.CloseCustomsShipment;

// One-way: there is no Reopen and no Delete.
public sealed record CloseCustomsShipmentCommand(
    Guid Id)
    : IRequest<Result>;
