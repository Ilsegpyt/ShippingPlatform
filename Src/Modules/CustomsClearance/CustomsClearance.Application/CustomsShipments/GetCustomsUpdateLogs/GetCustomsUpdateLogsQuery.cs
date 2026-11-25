using BuildingBlocks.Application;
using CustomsClearance.Domain.Entities;
using MediatR;

namespace CustomsClearance.Application.CustomsShipments.GetCustomsUpdateLogs;

// On-demand history for the Latest update cell modal.
public sealed record GetCustomsUpdateLogsQuery(
    Guid ShipmentId)
    : IRequest<Result<IReadOnlyList<CustomsUpdateLog>>>;
