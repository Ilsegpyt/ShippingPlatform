using BuildingBlocks.Application;
using MediatR;
using Transporters.Domain.Entities;

namespace Transporters.Application.Ports.GetPortCoverageById;

public sealed record GetPortCoverageByIdQuery(
    Guid TransporterId,
    Guid PortCoverageId)
    : IRequest<Result<TransporterPortCoverage>>;
